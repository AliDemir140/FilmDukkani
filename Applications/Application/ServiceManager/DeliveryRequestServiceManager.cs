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
        // NOT: Algoritma Issue #19'da yazılacak
        public async Task PrepareTomorrowDeliveriesAsync()
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);

            // YARIN teslimat isteyen tüm request'leri getir
            var requests = await _deliveryRequestRepository
                .GetAllAsync(r => r.DeliveryDate.Date == tomorrow.Date && r.Status == 0);

            // Burada algoritmanın iskeleti kuruluyor
            foreach (var request in requests)
            {
                // 1. Üyenin listesinde kaç film gönderilebilir? (Membership Plan'a göre)
                // 2. Listede önceliğe göre sıradaki filmleri bul
                // 3. Depoda uygun film var mı? (şimdilik kontrol etmiyoruz)
                // 4. DeliveryRequestItem oluştur
            }

            // Algoritmanın kendisi bir sonraki issue'da (#19) hayata geçecek
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
