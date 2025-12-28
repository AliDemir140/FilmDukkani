using Application.DTOs.DeliveryRequestDTOs;
using Application.Interfaces;
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
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly ICourierRepository _courierRepository;

        public DeliveryRequestServiceManager(
            IDeliveryRequestRepository deliveryRequestRepository,
            IDeliveryRequestItemRepository deliveryRequestItemRepository,
            IMemberMovieListRepository memberMovieListRepository,
            IMemberMovieListItemRepository memberMovieListItemRepository,
            IMovieRepository movieRepository,
            IMemberRepository memberRepository,
            IMembershipPlanRepository membershipPlanRepository,
            IMovieCopyRepository movieCopyRepository,
            IDamagedMovieRepository damagedMovieRepository,
            IEmailService emailService,
            ISmsService smsService,
            ICourierRepository courierRepository)
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
            _emailService = emailService;
            _smsService = smsService;
            _courierRepository = courierRepository;
        }

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

        private async Task<bool> HasActiveRequestForListAsync(int listId)
        {
            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            if (list == null) return false;

            return await _deliveryRequestRepository.HasActiveRequestForListAsync(list.MemberId, listId);
        }

        private async Task<int> GetOpenDeliveredCountByMemberAsync(int memberId)
        {
            var openDelivered = await _deliveryRequestRepository
                .GetAllAsync(r => r.MemberId == memberId && r.Status == DeliveryStatus.Delivered);

            return openDelivered?.Count() ?? 0;
        }

        public async Task<int> CreateDeliveryRequestAsync(int memberId, int listId, DateTime deliveryDate)
        {
            if (!IsValidDeliveryDate(deliveryDate))
                return 0;

            var openDeliveredCount = await GetOpenDeliveredCountByMemberAsync(memberId);
            if (openDeliveredCount >= 2)
                return -2;

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

        public async Task<int> ApproveCancelAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return 0;

            if (request.Status != DeliveryStatus.CancelRequested)
                return -1;

            if (!IsCancelAllowed(request.DeliveryDate))
                return -2;

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

        private sealed class DeliveryCandidate
        {
            public int RequestId { get; set; }
            public int MovieId { get; set; }
            public int ListItemId { get; set; }
            public int Priority { get; set; }
            public DateTime AddedDate { get; set; }
        }

        private static string BuildTomorrowEmailSubject(DateTime deliveryDate)
        {
            return "Yarın teslim edilecek filmler";
        }

        private string BuildTomorrowEmailBody(Member member, DeliveryRequest request, string listName, List<string> movieTitles)
        {
            var lines = new List<string>
            {
                "Merhaba " + member.FirstName + " " + member.LastName + ",",
                "",
                "Teslimat tarihi: " + request.DeliveryDate.ToString("dd.MM.yyyy"),
                "Liste: " + listName,
                "",
                "Yarın teslim edilecek filmler:"
            };

            if (movieTitles.Count == 0)
            {
                lines.Add("Bu teslimat için uygun kopya bulunamadı.");
            }
            else
            {
                for (int i = 0; i < movieTitles.Count; i++)
                {
                    lines.Add((i + 1) + ") " + movieTitles[i]);
                }
            }

            lines.Add("");
            lines.Add("FilmDukkani");

            return string.Join(Environment.NewLine, lines);
        }

        private async Task SendTomorrowEmailAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return;

            var member = await _memberRepository.GetByIdAsync(request.MemberId);
            if (member == null) return;

            if (string.IsNullOrWhiteSpace(member.Email))
                return;

            var list = await _memberMovieListRepository.GetByIdAsync(request.MemberMovieListId);
            var listName = list?.Name ?? string.Empty;

            var items = await _deliveryRequestItemRepository.GetAllAsync(i => i.DeliveryRequestId == request.ID);
            if (items == null || !items.Any())
                return;

            var titles = new List<string>();

            foreach (var item in items)
            {
                var movie = await _movieRepository.GetByIdAsync(item.MovieId);
                if (movie != null && !string.IsNullOrWhiteSpace(movie.Title))
                    titles.Add(movie.Title);
            }

            var subject = BuildTomorrowEmailSubject(request.DeliveryDate);
            var body = BuildTomorrowEmailBody(member, request, listName, titles);

            await _emailService.SendAsync(member.Email, subject, body);
        }

        private async Task SendShippedSmsAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return;

            var member = await _memberRepository.GetByIdAsync(request.MemberId);
            if (member == null) return;

            var phone = (member.Phone ?? "").Trim();
            if (string.IsNullOrWhiteSpace(phone))
                return;

            var message =
                "Kurye yola cikti. Teslimat tarihi: " +
                request.DeliveryDate.ToString("dd.MM.yyyy") +
                ". FilmDukkani";

            await _smsService.SendAsync(phone, message);
        }

        public async Task PrepareTomorrowDeliveriesAsync()
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);

            var requests = await _deliveryRequestRepository
                .GetAllAsync(r => r.DeliveryDate.Date == tomorrow.Date && r.Status == DeliveryStatus.Pending);

            if (requests == null || !requests.Any())
                return;

            requests = requests
                .OrderBy(r => r.ID)
                .ToList();

            var requestMap = requests.ToDictionary(r => r.ID, r => r);

            var quotaByRequestId = new Dictionary<int, int>();
            var assignedCountByRequestId = new Dictionary<int, int>();
            var anyItemAddedByRequestId = new Dictionary<int, bool>();
            var candidates = new List<DeliveryCandidate>();

            foreach (var request in requests)
            {
                var openDeliveredCount = await GetOpenDeliveredCountByMemberAsync(request.MemberId);
                if (openDeliveredCount >= 2)
                    continue;

                var member = await _memberRepository.GetByIdAsync(request.MemberId);
                if (member == null) continue;

                var plan = await _membershipPlanRepository.GetByIdAsync(member.MembershipPlanId);
                if (plan == null) continue;

                int quota = plan.MaxChangePerMonth;
                if (quota <= 0) continue;

                quotaByRequestId[request.ID] = quota;
                assignedCountByRequestId[request.ID] = 0;
                anyItemAddedByRequestId[request.ID] = false;

                var listItems = await _memberMovieListItemRepository
                    .GetAllAsync(i => i.MemberMovieListId == request.MemberMovieListId);

                if (listItems == null || !listItems.Any())
                    continue;

                var planned = listItems
                    .OrderBy(i => i.Priority)
                    .ThenBy(i => i.AddedDate)
                    .Take(quota)
                    .ToList();

                foreach (var li in planned)
                {
                    candidates.Add(new DeliveryCandidate
                    {
                        RequestId = request.ID,
                        MovieId = li.MovieId,
                        ListItemId = li.ID,
                        Priority = li.Priority,
                        AddedDate = li.AddedDate
                    });
                }
            }

            if (!candidates.Any())
                return;

            var movieIds = candidates.Select(c => c.MovieId).Distinct().ToList();

            var allCopies = await _movieCopyRepository.GetAllAsync(c =>
                movieIds.Contains(c.MovieId) && c.IsAvailable && !c.IsDamaged);

            var copyQueues = allCopies
                .GroupBy(c => c.MovieId)
                .ToDictionary(g => g.Key, g => new Queue<MovieCopy>(g.OrderBy(x => x.ID)));

            var orderedCandidates = candidates
                .OrderBy(c => c.Priority)
                .ThenBy(c => c.AddedDate)
                .ThenBy(c => c.RequestId)
                .ToList();

            foreach (var cand in orderedCandidates)
            {
                if (!quotaByRequestId.TryGetValue(cand.RequestId, out var quota))
                    continue;

                if (!assignedCountByRequestId.TryGetValue(cand.RequestId, out var assigned))
                    continue;

                if (assigned >= quota)
                    continue;

                if (!copyQueues.TryGetValue(cand.MovieId, out var q) || q.Count == 0)
                    continue;

                var selectedCopy = q.Dequeue();

                selectedCopy.IsAvailable = false;
                await _movieCopyRepository.UpdateAsync(selectedCopy);

                var dri = new DeliveryRequestItem
                {
                    DeliveryRequestId = cand.RequestId,
                    MovieId = cand.MovieId,
                    MovieCopyId = selectedCopy.ID,
                    MemberMovieListItemId = cand.ListItemId,
                    IsReturned = false,
                    IsDamaged = false
                };

                await _deliveryRequestItemRepository.AddAsync(dri);

                assignedCountByRequestId[cand.RequestId] = assigned + 1;
                anyItemAddedByRequestId[cand.RequestId] = true;
            }

            foreach (var pair in anyItemAddedByRequestId)
            {
                if (!pair.Value)
                    continue;

                if (!requestMap.TryGetValue(pair.Key, out var req))
                    continue;

                req.Status = DeliveryStatus.Prepared;
                await _deliveryRequestRepository.UpdateAsync(req);

                await SendTomorrowEmailAsync(req.ID);
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
                        maxMoviesToShow = plan.MaxChangePerMonth;
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

            string? courierFullName = null;
            if (request.CourierId.HasValue)
            {
                var courier = await _courierRepository.GetByIdAsync(request.CourierId.Value);
                if (courier != null)
                    courierFullName = (courier.FirstName + " " + courier.LastName).Trim();
            }

            return new DeliveryRequestDto
            {
                Id = request.ID,
                MemberId = request.MemberId,
                MemberFullName = member != null ? $"{member.FirstName} {member.LastName}" : string.Empty,

                MemberPhone = (member?.Phone ?? "").Trim(),
                MemberAddressLine = member?.AddressLine,
                MemberCity = member?.City,
                MemberDistrict = member?.District,
                MemberPostalCode = member?.PostalCode,

                MemberMovieListId = request.MemberMovieListId,
                ListName = list?.Name ?? string.Empty,
                RequestedDate = request.RequestedDate,
                DeliveryDate = request.DeliveryDate,
                Status = request.Status,

                CourierId = request.CourierId,
                CourierFullName = courierFullName,

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

            await SendShippedSmsAsync(request.ID);

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

        public async Task<int> AssignCourierAsync(int requestId, int courierId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null) return 0;

            var courier = await _courierRepository.GetByIdAsync(courierId);
            if (courier == null || !courier.IsActive) return -1;

            if (request.Status != DeliveryStatus.Prepared && request.Status != DeliveryStatus.Shipped)
                return -2;

            request.CourierId = courierId;
            await _deliveryRequestRepository.UpdateAsync(request);

            return 1;
        }

        public async Task<List<DeliveryRequestDto>> GetCourierDeliveriesAsync(int courierId, DateTime date, DeliveryStatus? status)
        {
            var courier = await _courierRepository.GetByIdAsync(courierId);
            if (courier == null || !courier.IsActive)
                return new List<DeliveryRequestDto>();

            var target = date.Date;

            var requests = await _deliveryRequestRepository.GetAllAsync(r =>
                r.CourierId == courierId &&
                r.DeliveryDate.Date == target &&
                (
                    status == null
                        ? (r.Status == DeliveryStatus.Prepared ||
                           r.Status == DeliveryStatus.Shipped ||
                           r.Status == DeliveryStatus.Delivered ||
                           r.Status == DeliveryStatus.Completed)
                        : r.Status == status.Value
                ));

            var result = new List<DeliveryRequestDto>();

            foreach (var r in requests.OrderByDescending(x => x.ID))
            {
                var dto = await GetRequestDetailAsync(r.ID);
                if (dto != null) result.Add(dto);
            }

            return result;
        }
    }
}
