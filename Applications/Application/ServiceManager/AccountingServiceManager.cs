using Application.DTOs.AccountingDTOs;
using Application.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceManager
{
    public class AccountingServiceManager
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDeliveryRequestRepository _deliveryRequestRepository;
        private readonly IDeliveryRequestItemRepository _deliveryRequestItemRepository;
        private readonly IMovieCopyRepository _movieCopyRepository;
        private readonly IDamagedMovieRepository _damagedMovieRepository;

        public AccountingServiceManager(
            IMemberRepository memberRepository,
            IMembershipPlanRepository membershipPlanRepository,
            IMovieRepository movieRepository,
            ICategoryRepository categoryRepository,
            IDeliveryRequestRepository deliveryRequestRepository,
            IDeliveryRequestItemRepository deliveryRequestItemRepository,
            IMovieCopyRepository movieCopyRepository,
            IDamagedMovieRepository damagedMovieRepository)
        {
            _memberRepository = memberRepository;
            _membershipPlanRepository = membershipPlanRepository;
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _deliveryRequestRepository = deliveryRequestRepository;
            _deliveryRequestItemRepository = deliveryRequestItemRepository;
            _movieCopyRepository = movieCopyRepository;
            _damagedMovieRepository = damagedMovieRepository;
        }

        /// <summary>
        /// Belirli bir tarih aralığı için genel kar/zarar özeti.
        /// NOT: Şimdilik iskelet; hesaplama ileride doldurulacak.
        /// </summary>
        public async Task<ProfitLossSummaryDto> GetProfitLossSummaryAsync(DateTime startDate, DateTime endDate)
        {
            // TODO: Üyelik ücretleri + satın alma maliyetleri + bozuk film maliyetleri üzerinden hesaplama yapılacak.

            await Task.CompletedTask; // şimdilik boş

            return new ProfitLossSummaryDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = 0m,
                TotalCost = 0m,
                Profit = 0m
            };
        }

        /// <summary>
        /// Üye bazlı kar/zarar raporu.
        /// </summary>
        public async Task<List<MemberProfitReportDto>> GetMemberProfitReportAsync(DateTime startDate, DateTime endDate)
        {
            // TODO: Her üye için üyelikten gelen gelir + ona gönderilen filmlerle ilgili maliyet hesaplanacak.

            await Task.CompletedTask;

            return new List<MemberProfitReportDto>();
        }

        /// <summary>
        /// Film bazlı kar/zarar raporu.
        /// </summary>
        public async Task<List<MovieProfitReportDto>> GetMovieProfitReportAsync(DateTime startDate, DateTime endDate)
        {
            // TODO: Her film için kaç kere gönderildiği, getirdiği gelir ve maliyet hesaplanacak.

            await Task.CompletedTask;

            return new List<MovieProfitReportDto>();
        }

        /// <summary>
        /// Kategori bazlı kar/zarar raporu.
        /// </summary>
        public async Task<List<CategoryProfitReportDto>> GetCategoryProfitReportAsync(DateTime startDate, DateTime endDate)
        {
            // TODO: Kategoriye bağlı filmler üzerinden gelir/maliyet toplanacak.

            await Task.CompletedTask;

            return new List<CategoryProfitReportDto>();
        }
    }
}
