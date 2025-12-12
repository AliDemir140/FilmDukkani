using Application.Repositories;
using Domain.Entities;
using Application.DTOs.DeliveryRequestDTOs;
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

        // Bir üyenin teslimat isteği oluşturması
        public async Task<int> CreateDeliveryRequestAsync(int memberId, int listId, DateTime deliveryDate)
        {
            var request = new DeliveryRequest
            {
                MemberId = memberId,
                MemberMovieListId = listId,
                RequestedDate = DateTime.Now,
                DeliveryDate = deliveryDate,
                Status = DeliveryStatus.Pending

            };

            await _deliveryRequestRepository.AddAsync(request);

            return request.ID;
        }

        // DTO ile teslimat isteği oluşturma
        public async Task<int> CreateDeliveryRequestAsync(CreateDeliveryRequestDto dto)
        {
            return await CreateDeliveryRequestAsync(dto.MemberId, dto.MemberMovieListId, dto.DeliveryDate);
        }


        // Yarının teslimatlarını hazırlayan sipariş hazırlama algoritması

        public async Task PrepareTomorrowDeliveriesAsync()
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);

            // 1) YARIN teslimat isteyen PENDING (0) tüm request'leri getir
            var requests = await _deliveryRequestRepository
                .GetAllAsync(r => r.DeliveryDate.Date == tomorrow.Date && r.Status == DeliveryStatus.Pending);

            foreach (var request in requests)
            {
                bool anyItemAdded = false;
                // 2) Üyeyi getir
                var member = await _memberRepository.GetByIdAsync(request.MemberId);
                if (member == null)
                    continue;

                // 3) Üyelik planını getir
                var plan = await _membershipPlanRepository.GetByIdAsync(member.MembershipPlanId);
                if (plan == null)
                    continue;

                int maxMoviesToSend = plan.MaxMoviesPerMonth;

                // 4) Üyenin listesini getir
                var list = await _memberMovieListRepository.GetByIdAsync(request.MemberMovieListId);
                if (list == null)
                    continue;

                // 5) Liste itemlarını öncelik doğrultusunda sırala
                var listItems = await _memberMovieListItemRepository
                    .GetAllAsync(i => i.MemberMovieListId == list.ID);

                var sortedItems = listItems
                    .OrderBy(i => i.Priority)       // Öncelik küçük olan önce gelir
                    .ThenBy(i => i.AddedDate)       // Aynı öncelikte eski olan gider
                    .Take(maxMoviesToSend)          // Üyelik planına göre gönderilecek film sayısı
                    .ToList();

                // 6) Bu filmleri DeliveryRequestItem olarak ekle
                foreach (var item in sortedItems)
                {

                    // 1) Bu MovieId için depoda uygun bir kopya bul
                    var copies = await _movieCopyRepository
                        .GetAllAsync(c => c.MovieId == item.MovieId && c.IsAvailable && !c.IsDamaged);

                    var selectedCopy = copies.FirstOrDefault();
                    if (selectedCopy == null)
                        continue; // stok yoksa bu filmi atla

                    // 2) Kopyayı rezerve et (başkasına gitmesin)
                    selectedCopy.IsAvailable = false;
                    await _movieCopyRepository.UpdateAsync(selectedCopy);

                    // 3) DeliveryRequestItem oluştur
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


                // 7) Request durumunu güncelle -> Prepared (1)
                if (anyItemAdded)
                {
                    request.Status = DeliveryStatus.Prepared;
                    await _deliveryRequestRepository.UpdateAsync(request);
                }
            }
        }


        // Request'i getir
        public async Task<DeliveryRequest?> GetRequestAsync(int requestId)
        {
            return await _deliveryRequestRepository.GetByIdAsync(requestId);
        }

        // Request'i detay DTO olarak getir
        public async Task<DeliveryRequestDto?> GetRequestDetailAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                return null;

            var member = await _memberRepository.GetByIdAsync(request.MemberId);
            var list = await _memberMovieListRepository.GetByIdAsync(request.MemberMovieListId);

            var items = await _deliveryRequestItemRepository
                .GetAllAsync(i => i.DeliveryRequestId == request.ID);

            var itemDtos = new List<DeliveryRequestItemDto>();

            foreach (var item in items)
            {
                var movie = await _movieRepository.GetByIdAsync(item.MovieId);

                itemDtos.Add(new DeliveryRequestItemDto
                {
                    Id = item.ID,
                    MovieId = item.MovieId,
                    MovieTitle = movie?.Title, // film bulunamazsa null kalır
                    IsReturned = item.IsReturned,
                    IsDamaged = item.IsDamaged,
                    ReturnDate = item.ReturnDate
                });
            }

            return new DeliveryRequestDto
            {
                Id = request.ID,
                MemberId = request.MemberId,
                MemberFullName = member != null
                    ? $"{member.FirstName} {member.LastName}"
                    : string.Empty,
                MemberMovieListId = request.MemberMovieListId,
                ListName = list?.Name,
                RequestedDate = request.RequestedDate,
                DeliveryDate = request.DeliveryDate,
                Status = request.Status,
                Items = itemDtos
            };
        }


        // Request iptal et
        public async Task<bool> CancelRequestAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                return false;

            request.Status = DeliveryStatus.Cancelled;   // Cancelled
            await _deliveryRequestRepository.UpdateAsync(request);

            return true;
        }

        // Request tamamlandı olarak işaretle
        public async Task<bool> MarkDeliveredAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                return false;

            request.Status = DeliveryStatus.Completed;
            await _deliveryRequestRepository.UpdateAsync(request);

            return true;
        }
        public async Task<bool> ReturnDeliveryItemAsync(ReturnDeliveryItemDto dto)
        {
            var item = await _deliveryRequestItemRepository.GetByIdAsync(dto.DeliveryRequestItemId);
            if (item == null)
                return false;

            if (item.IsReturned)
                return true;

            item.IsReturned = true;
            item.IsDamaged = dto.IsDamaged;
            item.ReturnDate = DateTime.Now;

            await _deliveryRequestItemRepository.UpdateAsync(item);

            // MovieCopy stok durumunu geri aç / bozuksa işaretle
            var copy = await _movieCopyRepository.GetByIdAsync(item.MovieCopyId);
            if (copy != null)
            {
                copy.IsDamaged = dto.IsDamaged;
                copy.IsAvailable = !dto.IsDamaged;
                await _movieCopyRepository.UpdateAsync(copy);
            }

            // Bozuksa DamagedMovie kaydı aç
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

            return true;
        }

    }
}
