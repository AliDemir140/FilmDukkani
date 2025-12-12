using Application.DTOs.AccountingReportDTOs;
using Application.Repositories;
using Domain.Enums;

namespace Application.ServiceManager
{
    public class AccountingReportServiceManager
    {
        private readonly IDeliveryRequestRepository _deliveryRequestRepository;
        private readonly IDeliveryRequestItemRepository _deliveryRequestItemRepository;
        private readonly IDamagedMovieRepository _damagedMovieRepository;
        private readonly IMovieCopyRepository _movieCopyRepository;
        private readonly IMovieRepository _movieRepository;

        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public AccountingReportServiceManager(
            IDeliveryRequestRepository deliveryRequestRepository,
            IDeliveryRequestItemRepository deliveryRequestItemRepository,
            IDamagedMovieRepository damagedMovieRepository,
            IMovieCopyRepository movieCopyRepository,
            IMovieRepository movieRepository,
            IMemberRepository memberRepository,
            IMembershipPlanRepository membershipPlanRepository)
        {
            _deliveryRequestRepository = deliveryRequestRepository;
            _deliveryRequestItemRepository = deliveryRequestItemRepository;
            _damagedMovieRepository = damagedMovieRepository;
            _movieCopyRepository = movieCopyRepository;
            _movieRepository = movieRepository;
            _memberRepository = memberRepository;
            _membershipPlanRepository = membershipPlanRepository;
        }

        /// <summary>
        /// Temel muhasebe özeti (teslimat + iade + bozuk + üyelikten tahmini gelir)
        /// </summary>
        public async Task<AccountingSummaryDto> GetSummaryAsync(DateTime from, DateTime to)
        {
            // Guard
            if (to < from)
                (from, to) = (to, from);

            // DeliveryRequest'ler (tarih aralığı: RequestedDate veya DeliveryDate? -> burada DeliveryDate baz alıyoruz)
            var requests = await _deliveryRequestRepository
                .GetAllAsync(r => r.DeliveryDate.Date >= from.Date && r.DeliveryDate.Date <= to.Date);

            int total = requests.Count;
            int prepared = requests.Count(r => r.Status == DeliveryStatus.Prepared);
            int completed = requests.Count(r => r.Status == DeliveryStatus.Completed);
            int cancelled = requests.Count(r => r.Status == DeliveryStatus.Cancelled);

            // RequestItem'lar (o request'lere bağlı item sayıları)
            // Not: repo generic olduğundan Include yok; bu yüzden requestId listesi ile filtreliyoruz
            var requestIds = requests.Select(r => r.ID).ToList();

            var items = await _deliveryRequestItemRepository
                .GetAllAsync(i => requestIds.Contains(i.DeliveryRequestId));

            int totalItems = items.Count;
            int returnedItems = items.Count(i => i.IsReturned);
            int damagedItems = items.Count(i => i.IsDamaged);

            // Üyelikten tahmini gelir:
            // Basit yaklaşım: tarih aralığında aktif üyelerin plan fiyatlarını toplar.
            // (WORLD’de aylık mı, başlangıç tarihi mi vs. varsa sonra refine ederiz.)
            var members = await _memberRepository.GetAllAsync(m =>
                          m.MembershipStartDate.Date <= to.Date);


            decimal revenue = 0m;
            foreach (var member in members)
            {
                var plan = await _membershipPlanRepository.GetByIdAsync(member.MembershipPlanId);
                if (plan != null)
                    revenue += plan.Price;
            }

            return new AccountingSummaryDto
            {
                From = from.Date,
                To = to.Date,

                TotalDeliveryRequests = total,
                PreparedDeliveryRequests = prepared,
                CompletedDeliveryRequests = completed,
                CancelledDeliveryRequests = cancelled,

                TotalDeliveredItems = totalItems,
                ReturnedItems = returnedItems,
                DamagedItems = damagedItems,

                EstimatedMembershipRevenue = revenue
            };
        }

        /// <summary>
        /// Bozuk film raporu (DamagedMovie kayıtları üzerinden)
        /// </summary>
        public async Task<List<DamagedMoviesReportItemDto>> GetDamagedMoviesReportAsync()
        {
            var damagedRecords = await _damagedMovieRepository.GetAllAsync();

            var result = new List<DamagedMoviesReportItemDto>();

            foreach (var d in damagedRecords)
            {
                // MovieCopy -> MovieId
                var copy = await _movieCopyRepository.GetByIdAsync(d.MovieCopyId);
                int movieId = copy?.MovieId ?? 0;

                string? title = null;
                if (movieId != 0)
                {
                    var movie = await _movieRepository.GetByIdAsync(movieId);
                    title = movie?.Title;
                }

                result.Add(new DamagedMoviesReportItemDto
                {
                    DamagedMovieId = d.ID,
                    MovieCopyId = d.MovieCopyId,
                    MovieId = movieId,
                    MovieTitle = title,
                    IsSentToPurchase = d.IsSentToPurchase,
                    Note = d.Note
                });
            }

            return result;
        }
    }
}
