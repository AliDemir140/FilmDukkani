using Application.Repositories;
using Domain.Entities;

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

        public DeliveryRequestServiceManager(
            IDeliveryRequestRepository deliveryRequestRepository,
            IDeliveryRequestItemRepository deliveryRequestItemRepository,
            IMemberMovieListRepository memberMovieListRepository,
            IMemberMovieListItemRepository memberMovieListItemRepository,
            IMovieRepository movieRepository,
            IMemberRepository memberRepository,
            IMembershipPlanRepository membershipPlanRepository)
        {
            _deliveryRequestRepository = deliveryRequestRepository;
            _deliveryRequestItemRepository = deliveryRequestItemRepository;
            _memberMovieListRepository = memberMovieListRepository;
            _memberMovieListItemRepository = memberMovieListItemRepository;
            _movieRepository = movieRepository;
            _memberRepository = memberRepository;
            _membershipPlanRepository = membershipPlanRepository;
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
                Status = 0 // Pending
            };

            await _deliveryRequestRepository.AddAsync(request);

            return request.ID;
        }

        // Yarının siparişlerini hazırlamak için skeleton işlem
        public async Task PrepareTomorrowDeliveriesAsync()
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);

            // 1) YARIN teslimat isteyen PENDING (0) tüm request'leri getir
            var requests = await _deliveryRequestRepository
                .GetAllAsync(r => r.DeliveryDate.Date == tomorrow.Date && r.Status == 0);

            foreach (var request in requests)
            {
                // 2) Üyeyi getir
                var member = await _memberRepository.GetByIdAsync(request.MemberId);
                if (member == null)
                    continue;

                // 3) Üyelik planını getir
                var plan = await _membershipPlanRepository.GetByIdAsync(member.MembershipPlanId);
                if (plan == null)
                    continue;

                int maxMoviesToSend = plan.MaxMoviesPerMonth;
                // Not: WORLD.doc'ta "bir değişimde X film" geçer — o değer plan tablosunda olacak

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
                    var dri = new DeliveryRequestItem
                    {
                        DeliveryRequestId = request.ID,
                        MovieId = item.MovieId,
                        MemberMovieListItemId = item.ID,
                        IsReturned = false,
                        IsDamaged = false
                    };

                    await _deliveryRequestItemRepository.AddAsync(dri);
                }

                // 7) Request durumunu güncelle -> Prepared (1)
                request.Status = 1;
                await _deliveryRequestRepository.UpdateAsync(request);
            }
        }


        // Request'i getir
        public async Task<DeliveryRequest?> GetRequestAsync(int requestId)
        {
            return await _deliveryRequestRepository.GetByIdAsync(requestId);
        }

        // Request iptal et
        public async Task<bool> CancelRequestAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                return false;

            request.Status = 5; // Cancelled
            await _deliveryRequestRepository.UpdateAsync(request);

            return true;
        }

        // Request tamamlandı olarak işaretle
        public async Task<bool> MarkDeliveredAsync(int requestId)
        {
            var request = await _deliveryRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                return false;

            request.Status = 4; // Completed
            await _deliveryRequestRepository.UpdateAsync(request);

            return true;
        }
    }
}
