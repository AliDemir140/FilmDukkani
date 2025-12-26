using Application.DTOs.DeliveryRequestDTOs;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;

namespace Application.ServiceManager
{
    public class DeliveryRequestServiceManager
    {
        private readonly IDeliveryRequestRepository _deliveryRequestRepository;
        private readonly IDeliveryRequestItemRepository _deliveryRequestItemRepository;
        private readonly IMemberMovieListRepository _memberMovieListRepository;
        private readonly IMemberMovieListItemRepository _memberMovieListItemRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;
        private readonly IMovieCopyRepository _movieCopyRepository;
        private readonly IDamagedMovieRepository _damagedMovieRepository;

        public DeliveryRequestServiceManager(
            IDeliveryRequestRepository deliveryRequestRepository,
            IDeliveryRequestItemRepository deliveryRequestItemRepository,
            IMemberMovieListRepository memberMovieListRepository,
            IMemberMovieListItemRepository memberMovieListItemRepository,
            IMovieRepository movieRepository,
            IMemberRepository memberRepository,
            IMembershipPlanRepository membershipPlanRepository,
            IMovieCopyRepository movieCopyRepository,
            IDamagedMovieRepository damagedMovieRepository)
        {
            _deliveryRequestRepository = deliveryRequestRepository;
            _deliveryRequestItemRepository = deliveryRequestItemRepository;
            _memberMovieListRepository = memberMovieListRepository;
            _memberMovieListItemRepository = memberMovieListItemRepository;
            _movieRepository = movieRepository;
            _memberRepository = memberRepository;
            _membershipPlanRepository = membershipPlanRepository;
            _movieCopyRepository = movieCopyRepository;
            _damagedMovieRepository = damagedMovieRepository;
        }

        // =========================================================
        // ✅ CONTROLLER UYUMLULUK (CS1061 fix)
        // Controller bunları çağırıyor:
        //   UserCancelRequestAsync(memberId, requestId, reason)
        //   AdminDecideCancelAsync(requestId, approve)
        // Bizdeki mevcut metotlara map’liyoruz.
        // =========================================================
        public async Task<int> UserCancelRequestAsync(int memberId, int requestId, string reason)
        {
            return await RequestCancelAsync(requestId, memberId, reason);
        }

        public async Task<int> AdminDecideCancelAsync(int requestId, bool approve)
        {
            return approve
                ? await ApproveCancelAsync(requestId)
                : await RejectCancelAsync(requestId);
        }

        // Teslimat tarihi: en az 2 gün sonrası, pazar olamaz
        private static bool IsValidDeliveryDate(DateTime deliveryDate)
        {
            var today = DateTime.Today;
            var target = deliveryDate.Date;

            if (target < today.AddDays(2)) return false;
            if (target.DayOfWeek == DayOfWeek.Sunday) return false;

            return true;
        }

        // İptal: en geç 1 gün öncesine kadar
        private static bool IsCancelAllowed(DateTime deliveryDate)
        {
            return DateTime.Today <= deliveryDate.Date.AddDays(-1);
        }

        // KURAL: aynı liste için aktif sipariş varken yeni sipariş açılmasın
        private async Task<bool> HasActiveRequestForListAsync(int listId)
        {
            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            if (list == null) return false;

            return await _deliveryRequestRepository.HasActiveRequestForListAsync(list.MemberId, listId);
        }

        // ✅ Teslimat isteği oluştur
        // Return:
        //  0  -> geçersiz tarih
        // -1  -> aynı liste için aktif sipariş var
        // >0  -> requestId
        public async Task<int> CreateDeliveryRequestAsync(int memberId, int listId, DateTime deliveryDate)
        {
            if (!IsValidDeliveryDate(deliveryDate))
                return 0;

            if (await HasActiveRequestForListAsync(listId))
                return -1;

            var request = new DeliveryRequest
            {
                MemberId = memberId,
                MemberMovieListId = listId,
                RequestedDate = DateTime.Now,
                DeliveryDate = deliveryDate.Date,
                Status = DeliveryStatus.Pending
            };

            await _deliveryRequestRepository.AddAsync(request);
            return request.ID;
        }

        public async Task<int> CreateDeliveryRequestAsync(CreateDeliveryRequestDto dto)
        {
            return await CreateDeliveryRequestAsync(dto.MemberId, dto.MemberMovieListId, dto.DeliveryDate);
        }

        // ✅ KULLANICI: iptal talebi açar -> Status = CancelRequested
        // Return:
        //  0  -> request yok
        // -1  -> yetkisiz
        // -2  -> statü uygun değil (Completed/Cancelled)
        // -3  -> zaten CancelRequested
        // -4  -> iptal tarihi kuralı (en geç 1 gün önce)
        //  1  -> ok
        public async Task<int> RequestCancelAsync(int requestId, int memberId, string reason)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return 0;

            if (request.MemberId != memberId) return -1;

            if (request.Status == DeliveryStatus.Completed || request.Status == DeliveryStatus.Cancelled)
                return -2;

            if (request.Status == DeliveryStatus.CancelRequested)
                return -3;

            if (!IsCancelAllowed(request.DeliveryDate))
                return -4;

            reason = (reason ?? "").Trim();
            if (string.IsNullOrWhiteSpace(reason))
                reason = "Kullanıcı iptal talebi oluşturdu.";

            request.CancelPreviousStatus = request.Status;
            request.CancelReason = reason;
            request.CancelRequestedAt = DateTime.Now;
            request.CancelApproved = null;
            request.CancelDecisionAt = null;

            request.Status = DeliveryStatus.CancelRequested;

            await _deliveryRequestRepository.UpdateAsync(request);
            return 1;
        }

        // ✅ ADMIN: iptal talebini ONAYLA -> Cancelled
        // Return:
        //  0 -> request yok
        // -1 -> CancelRequested değil
        // -2 -> iptal tarihi kuralı
        //  1 -> ok
        public async Task<int> ApproveCancelAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return 0;

            if (request.Status != DeliveryStatus.CancelRequested)
                return -1;

            if (!IsCancelAllowed(request.DeliveryDate))
                return -2;

            // Prepared ve sonrası: item/copy varsa geri aç + itemları sil
            var items = await _deliveryRequestItemRepository
                .GetAllAsync(i => i.DeliveryRequestId == request.ID);

            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    var copy = await _movieCopyRepository.GetByIdAsync(item.MovieCopyId);
                    if (copy != null && !copy.IsDamaged)
                    {
                        copy.IsAvailable = true;
                        await _movieCopyRepository.UpdateAsync(copy);
                    }

                    await _deliveryRequestItemRepository.DeleteAsync(item);
                }
            }

            request.Status = DeliveryStatus.Cancelled;
            request.CancelApproved = true;
            request.CancelDecisionAt = DateTime.Now;

            await _deliveryRequestRepository.UpdateAsync(request);
            return 1;
        }

        // ADMIN: iptal talebini REDDET -> eski statüye dön
        // Return:
        //  0 -> request yok
        // -1 -> CancelRequested değil
        //  1 -> ok
        public async Task<int> RejectCancelAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return 0;

            if (request.Status != DeliveryStatus.CancelRequested)
                return -1;

            request.CancelApproved = false;
            request.CancelDecisionAt = DateTime.Now;

            var back = request.CancelPreviousStatus ?? DeliveryStatus.Pending;
            request.Status = back;

            await _deliveryRequestRepository.UpdateAsync(request);
            return 1;
        }

        public async Task PrepareTomorrowDeliveriesAsync()
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);

            var requests = await _deliveryRequestRepository
                .GetAllAsync(r => r.DeliveryDate.Date == tomorrow.Date && r.Status == DeliveryStatus.Pending);

            foreach (var request in requests)
            {
                bool anyItemAdded = false;

                var member = await _memberRepository.GetByIdAsync(request.MemberId);
                if (member == null) continue;

                var plan = await _membershipPlanRepository.GetByIdAsync(member.MembershipPlanId);
                if (plan == null) continue;

                int maxMoviesToSend = plan.MaxMoviesPerMonth;

                var list = await _memberMovieListRepository.GetByIdAsync(request.MemberMovieListId);
                if (list == null) continue;

                var listItems = await _memberMovieListItemRepository
                    .GetAllAsync(i => i.MemberMovieListId == list.ID);

                var sortedItems = listItems
                    .OrderBy(i => i.Priority)
                    .ThenBy(i => i.AddedDate)
                    .Take(maxMoviesToSend)
                    .ToList();

                foreach (var item in sortedItems)
                {
                    var copies = await _movieCopyRepository
                        .GetAllAsync(c => c.MovieId == item.MovieId && c.IsAvailable && !c.IsDamaged);

                    var selectedCopy = copies.FirstOrDefault();
                    if (selectedCopy == null) continue;

                    selectedCopy.IsAvailable = false;
                    await _movieCopyRepository.UpdateAsync(selectedCopy);

                    var dri = new DeliveryRequestItem
                    {
                        DeliveryRequestId = request.ID,
                        MovieId = item.MovieId,
                        MovieCopyId = selectedCopy.ID,
                        MemberMovieListItemId = item.ID,
                        IsReturned = false,
                        IsDamaged = false
                    };

                    await _deliveryRequestItemRepository.AddAsync(dri);
                    anyItemAdded = true;
                }

                if (anyItemAdded)
                {
                    request.Status = DeliveryStatus.Prepared;
                    await _deliveryRequestRepository.UpdateAsync(request);
                }
            }
        }

        public async Task<DeliveryRequestDto?> GetRequestDetailAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return null;

            var member = await _memberRepository.GetByIdAsync(request.MemberId);
            var list = await _memberMovieListRepository.GetByIdAsync(request.MemberMovieListId);

            var items = await _deliveryRequestItemRepository
                .GetAllAsync(i => i.DeliveryRequestId == request.ID);

            var itemDtos = new List<DeliveryRequestItemDto>();

            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    var movie = await _movieRepository.GetByIdAsync(item.MovieId);

                    itemDtos.Add(new DeliveryRequestItemDto
                    {
                        Id = item.ID,
                        MovieId = item.MovieId,
                        MovieTitle = movie?.Title,
                        IsReturned = item.IsReturned,
                        IsDamaged = item.IsDamaged,
                        ReturnDate = item.ReturnDate
                    });
                }
            }
            else
            {
                int maxMoviesToShow = 0;

                if (member != null)
                {
                    var plan = await _membershipPlanRepository.GetByIdAsync(member.MembershipPlanId);
                    if (plan != null)
                        maxMoviesToShow = plan.MaxMoviesPerMonth;
                }

                if (maxMoviesToShow <= 0)
                    maxMoviesToShow = 5;

                var listItems = await _memberMovieListItemRepository
                    .GetAllAsync(i => i.MemberMovieListId == request.MemberMovieListId);

                var planned = listItems
                    .OrderBy(i => i.Priority)
                    .ThenBy(i => i.AddedDate)
                    .Take(maxMoviesToShow)
                    .ToList();

                foreach (var li in planned)
                {
                    var movie = await _movieRepository.GetByIdAsync(li.MovieId);

                    itemDtos.Add(new DeliveryRequestItemDto
                    {
                        Id = 0,
                        MovieId = li.MovieId,
                        MovieTitle = movie?.Title,
                        IsReturned = false,
                        IsDamaged = false,
                        ReturnDate = null
                    });
                }
            }

            return new DeliveryRequestDto
            {
                Id = request.ID,
                MemberId = request.MemberId,
                MemberFullName = member != null ? $"{member.FirstName} {member.LastName}" : string.Empty,
                MemberMovieListId = request.MemberMovieListId,
                ListName = list?.Name ?? string.Empty,
                RequestedDate = request.RequestedDate,
                DeliveryDate = request.DeliveryDate,
                Status = request.Status,

                CancelReason = request.CancelReason,
                CancelRequestedAt = request.CancelRequestedAt,
                CancelApproved = request.CancelApproved,
                CancelDecisionAt = request.CancelDecisionAt,
                CancelPreviousStatus = request.CancelPreviousStatus,

                Items = itemDtos
            };
        }

        public async Task<List<DeliveryRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _deliveryRequestRepository.GetAllAsync();
            var result = new List<DeliveryRequestDto>();

            foreach (var r in requests.OrderByDescending(x => x.ID))
            {
                var dto = await GetRequestDetailAsync(r.ID);
                if (dto != null) result.Add(dto);
            }

            return result;
        }

        public async Task<List<DeliveryRequestDto>> GetRequestsByStatusAsync(DeliveryStatus status)
        {
            var requests = await _deliveryRequestRepository.GetAllAsync(r => r.Status == status);
            var result = new List<DeliveryRequestDto>();

            foreach (var r in requests.OrderByDescending(x => x.ID))
            {
                var dto = await GetRequestDetailAsync(r.ID);
                if (dto != null) result.Add(dto);
            }

            return result;
        }

        // Admin tarafında direkt iptal (eski davranış) istersen kalsın:
        public async Task<bool> CancelRequestAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return false;

            if (request.Status != DeliveryStatus.Pending && request.Status != DeliveryStatus.Prepared)
                return false;

            if (!IsCancelAllowed(request.DeliveryDate))
                return false;

            var items = await _deliveryRequestItemRepository
                .GetAllAsync(i => i.DeliveryRequestId == request.ID);

            foreach (var item in items)
            {
                var copy = await _movieCopyRepository.GetByIdAsync(item.MovieCopyId);
                if (copy != null && !copy.IsDamaged)
                {
                    copy.IsAvailable = true;
                    await _movieCopyRepository.UpdateAsync(copy);
                }

                await _deliveryRequestItemRepository.DeleteAsync(item);
            }

            request.Status = DeliveryStatus.Cancelled;
            await _deliveryRequestRepository.UpdateAsync(request);

            return true;
        }

        public async Task<bool> MarkShippedAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return false;

            if (request.Status != DeliveryStatus.Prepared)
                return false;

            request.Status = DeliveryStatus.Shipped;
            await _deliveryRequestRepository.UpdateAsync(request);

            return true;
        }

        public async Task<bool> MarkDeliveredAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return false;

            if (request.Status != DeliveryStatus.Shipped)
                return false;

            var items = await _deliveryRequestItemRepository
                .GetAllAsync(i => i.DeliveryRequestId == request.ID);

            foreach (var item in items)
            {
                var listItem = await _memberMovieListItemRepository.GetByIdAsync(item.MemberMovieListItemId);
                if (listItem != null)
                    await _memberMovieListItemRepository.DeleteAsync(listItem);
            }

            request.Status = DeliveryStatus.Delivered;
            await _deliveryRequestRepository.UpdateAsync(request);

            return true;
        }

        public async Task<bool> MarkCompletedAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return false;

            if (request.Status != DeliveryStatus.Delivered)
                return false;

            var items = await _deliveryRequestItemRepository
                .GetAllAsync(i => i.DeliveryRequestId == request.ID);

            if (items.Any(i => !i.IsReturned))
                return false;

            request.Status = DeliveryStatus.Completed;
            await _deliveryRequestRepository.UpdateAsync(request);

            return true;
        }

        public async Task<bool> ReturnDeliveryItemAsync(ReturnDeliveryItemDto dto)
        {
            var item = await _deliveryRequestItemRepository.GetByIdAsync(dto.DeliveryRequestItemId);
            if (item == null) return false;

            if (item.IsReturned) return true;

            item.IsReturned = true;
            item.IsDamaged = dto.IsDamaged;
            item.ReturnDate = DateTime.Now;
            await _deliveryRequestItemRepository.UpdateAsync(item);

            var copy = await _movieCopyRepository.GetByIdAsync(item.MovieCopyId);
            if (copy != null)
            {
                copy.IsDamaged = dto.IsDamaged;
                copy.IsAvailable = !dto.IsDamaged;
                await _movieCopyRepository.UpdateAsync(copy);
            }

            if (dto.IsDamaged)
            {
                var damaged = new DamagedMovie
                {
                    MovieCopyId = item.MovieCopyId,
                    Note = dto.Note,
                    IsSentToPurchase = false
                };

                await _damagedMovieRepository.AddAsync(damaged);
            }

            var request = await _deliveryRequestRepository.GetByIdAsync(item.DeliveryRequestId);
            if (request != null && request.Status == DeliveryStatus.Delivered)
            {
                var allItems = await _deliveryRequestItemRepository
                    .GetAllAsync(i => i.DeliveryRequestId == request.ID);

                if (allItems.All(i => i.IsReturned))
                {
                    request.Status = DeliveryStatus.Completed;
                    await _deliveryRequestRepository.UpdateAsync(request);
                }
            }

            return true;
        }

        public async Task<List<DeliveryRequestListDto>> GetRequestsByMemberAsync(int memberId)
        {
            return await _deliveryRequestRepository.GetByMemberAsync(memberId);
        }
    }
}
