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

        private static bool IsValidDeliveryDate(DateTime deliveryDate)
        {
            var today = DateTime.Today;
            var target = deliveryDate.Date;

            if (target < today.AddDays(2)) return false;
            if (target.DayOfWeek == DayOfWeek.Sunday) return false;

            return true;
        }

        private static bool IsCancelAllowed(DateTime deliveryDate)
        {
            return DateTime.Today <= deliveryDate.Date.AddDays(-1);
        }

        // ✅ KURAL: aynı listeden aktif sipariş varken yeni sipariş açılmasın
        private async Task<bool> HasActiveRequestForListAsync(int listId)
        {
            var listRequests = await _deliveryRequestRepository.GetAllAsync(r =>
                r.MemberMovieListId == listId &&
                r.Status != DeliveryStatus.Cancelled &&
                r.Status != DeliveryStatus.Completed
            );

            return listRequests.Any();
        }

        public async Task<int> CreateDeliveryRequestAsync(int memberId, int listId, DateTime deliveryDate)
        {
            if (!IsValidDeliveryDate(deliveryDate))
                return 0;

            // ✅ aktif sipariş kontrolü
            if (await HasActiveRequestForListAsync(listId))
                return 0;

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

        // ---- AŞAĞISI SENİN KODUN (DEĞİŞMEDİ) ----

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

        public async Task<bool> CancelRequestAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return false;

            if (request.Status != DeliveryStatus.Pending && request.Status != DeliveryStatus.Prepared)
                return false;

            if (!IsCancelAllowed(request.DeliveryDate))
                return false;

            if (request.Status == DeliveryStatus.Prepared)
            {
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
