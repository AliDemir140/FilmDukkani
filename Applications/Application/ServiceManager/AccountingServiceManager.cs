using Application.DTOs.AccountingDTOs;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private const decimal ESTIMATED_DAMAGE_UNIT_COST = 50m;
        private const decimal ESTIMATED_DELIVERY_UNIT_COST = 0m;

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

        public async Task<ProfitLossSummaryDto> GetProfitLossSummaryAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                (startDate, endDate) = (endDate, startDate);

            startDate = startDate.Date;
            endDate = endDate.Date;

            var members = await _memberRepository.GetAllAsync(m => m.MembershipStartDate.Date <= endDate);

            var plans = await _membershipPlanRepository.GetAllAsync();
            var planById = plans.ToDictionary(p => p.ID, p => p);

            var requests = await _deliveryRequestRepository.GetAllAsync(r =>
                r.DeliveryDate.Date >= startDate &&
                r.DeliveryDate.Date <= endDate &&
                (r.Status == DeliveryStatus.Prepared || r.Status == DeliveryStatus.Completed));

            var requestIds = new HashSet<int>(requests.Select(r => r.ID));

            var allItems = await _deliveryRequestItemRepository.GetAllAsync();
            var itemsInRange = allItems.Where(i => requestIds.Contains(i.DeliveryRequestId)).ToList();

            var damaged = await _damagedMovieRepository.GetAllAsync();
            var damagedInRange = damaged.Where(d => d.CreatedDate.Date >= startDate && d.CreatedDate.Date <= endDate).ToList();

            decimal totalRevenue = 0m;

            foreach (var m in members)
            {
                if (!planById.TryGetValue(m.MembershipPlanId, out var plan))
                    continue;

                var effectiveStart = m.MembershipStartDate.Date > startDate ? m.MembershipStartDate.Date : startDate;

                int months = MonthsInclusive(effectiveStart, endDate);
                if (months < 0) months = 0;

                totalRevenue += (plan.Price * months);
            }

            decimal totalCost =
                (damagedInRange.Count * ESTIMATED_DAMAGE_UNIT_COST) +
                (itemsInRange.Count * ESTIMATED_DELIVERY_UNIT_COST);

            return new ProfitLossSummaryDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                Profit = totalRevenue - totalCost
            };
        }

        public async Task<List<MemberProfitReportDto>> GetMemberProfitReportAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                (startDate, endDate) = (endDate, startDate);

            startDate = startDate.Date;
            endDate = endDate.Date;

            var members = await _memberRepository.GetAllAsync(m => m.MembershipStartDate.Date <= endDate);

            var plans = await _membershipPlanRepository.GetAllAsync();
            var planById = plans.ToDictionary(p => p.ID, p => p);

            var requests = await _deliveryRequestRepository.GetAllAsync(r =>
                r.DeliveryDate.Date >= startDate &&
                r.DeliveryDate.Date <= endDate &&
                (r.Status == DeliveryStatus.Prepared || r.Status == DeliveryStatus.Completed));

            var requestById = requests.ToDictionary(r => r.ID, r => r);

            var allItems = await _deliveryRequestItemRepository.GetAllAsync();
            var itemsInRange = allItems.Where(i => requestById.ContainsKey(i.DeliveryRequestId)).ToList();

            var itemsByMemberId = itemsInRange
                .GroupBy(i => requestById[i.DeliveryRequestId].MemberId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<MemberProfitReportDto>();

            foreach (var m in members)
            {
                planById.TryGetValue(m.MembershipPlanId, out var plan);

                var effectiveStart = m.MembershipStartDate.Date > startDate ? m.MembershipStartDate.Date : startDate;
                int months = MonthsInclusive(effectiveStart, endDate);
                if (months < 0) months = 0;

                decimal revenue = (plan != null) ? plan.Price * months : 0m;

                itemsByMemberId.TryGetValue(m.ID, out var memberItems);
                memberItems ??= new List<DeliveryRequestItem>();

                int deliveredCount = memberItems.Count;
                int damagedCount = memberItems.Count(x => x.IsDamaged);

                decimal cost =
                    (damagedCount * ESTIMATED_DAMAGE_UNIT_COST) +
                    (deliveredCount * ESTIMATED_DELIVERY_UNIT_COST);

                result.Add(new MemberProfitReportDto
                {
                    MemberId = m.ID,
                    MemberFullName = $"{m.FirstName} {m.LastName}",
                    MembershipPlanId = m.MembershipPlanId,
                    MembershipPlanName = plan?.PlanName,
                    DeliveredMovieCount = deliveredCount,
                    DamagedMovieCount = damagedCount,
                    TotalRevenue = revenue,
                    TotalCost = cost,
                    Profit = revenue - cost
                });
            }

            return result
                .OrderByDescending(x => x.Profit)
                .ToList();
        }

        public async Task<List<MovieProfitReportDto>> GetMovieProfitReportAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                (startDate, endDate) = (endDate, startDate);

            startDate = startDate.Date;
            endDate = endDate.Date;

            var requests = await _deliveryRequestRepository.GetAllAsync(r =>
                r.DeliveryDate.Date >= startDate &&
                r.DeliveryDate.Date <= endDate &&
                (r.Status == DeliveryStatus.Prepared || r.Status == DeliveryStatus.Completed));

            var requestIds = new HashSet<int>(requests.Select(r => r.ID));

            var allItems = await _deliveryRequestItemRepository.GetAllAsync();
            var itemsInRange = allItems.Where(i => requestIds.Contains(i.DeliveryRequestId)).ToList();

            var movies = await _movieRepository.GetAllAsync();

            var categories = await _categoryRepository.GetAllAsync();
            var categoryById = categories.ToDictionary(c => c.ID, c => c.CategoryName);

            var copies = await _movieCopyRepository.GetAllAsync();
            var movieIdByCopyId = copies.ToDictionary(c => c.ID, c => c.MovieId);

            var damaged = await _damagedMovieRepository.GetAllAsync();
            var damagedInRange = damaged.Where(d => d.CreatedDate.Date >= startDate && d.CreatedDate.Date <= endDate).ToList();

            var deliveredByMovieId = itemsInRange
                .GroupBy(i => i.MovieId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var damagedCountByMovieId = new Dictionary<int, int>();
            foreach (var d in damagedInRange)
            {
                if (movieIdByCopyId.TryGetValue(d.MovieCopyId, out int movieId))
                {
                    if (!damagedCountByMovieId.ContainsKey(movieId))
                        damagedCountByMovieId[movieId] = 0;

                    damagedCountByMovieId[movieId]++;
                }
            }

            const decimal ESTIMATED_REVENUE_PER_DELIVERY_ITEM = 0m;

            var result = new List<MovieProfitReportDto>();

            foreach (var movie in movies)
            {
                deliveredByMovieId.TryGetValue(movie.ID, out var movieItems);
                movieItems ??= new List<DeliveryRequestItem>();

                int deliveredCount = movieItems.Count;
                int damagedCount = damagedCountByMovieId.TryGetValue(movie.ID, out var dc) ? dc : 0;

                int primaryCategoryId = GetPrimaryCategoryId(movie);
                string? primaryCategoryName = primaryCategoryId > 0 && categoryById.TryGetValue(primaryCategoryId, out var catName)
                    ? catName
                    : null;

                decimal revenue = deliveredCount * ESTIMATED_REVENUE_PER_DELIVERY_ITEM;
                decimal cost =
                    (damagedCount * ESTIMATED_DAMAGE_UNIT_COST) +
                    (deliveredCount * ESTIMATED_DELIVERY_UNIT_COST);

                result.Add(new MovieProfitReportDto
                {
                    MovieId = movie.ID,
                    MovieTitle = movie.Title,
                    CategoryId = primaryCategoryId,
                    CategoryName = primaryCategoryName,
                    DeliveredCount = deliveredCount,
                    DamagedCount = damagedCount,
                    TotalRevenue = revenue,
                    TotalCost = cost,
                    Profit = revenue - cost
                });
            }

            return result
                .OrderByDescending(x => x.DeliveredCount)
                .ToList();
        }

        public async Task<List<CategoryProfitReportDto>> GetCategoryProfitReportAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                (startDate, endDate) = (endDate, startDate);

            startDate = startDate.Date;
            endDate = endDate.Date;

            var categories = await _categoryRepository.GetAllAsync();
            var movies = await _movieRepository.GetAllAsync();
            var movieById = movies.ToDictionary(m => m.ID, m => m);

            var requests = await _deliveryRequestRepository.GetAllAsync(r =>
                r.DeliveryDate.Date >= startDate &&
                r.DeliveryDate.Date <= endDate &&
                (r.Status == DeliveryStatus.Prepared || r.Status == DeliveryStatus.Completed));

            var requestIds = new HashSet<int>(requests.Select(r => r.ID));

            var allItems = await _deliveryRequestItemRepository.GetAllAsync();
            var itemsInRange = allItems.Where(i => requestIds.Contains(i.DeliveryRequestId)).ToList();

            var deliveredByCategoryId = new Dictionary<int, int>();

            foreach (var item in itemsInRange)
            {
                if (!movieById.TryGetValue(item.MovieId, out var movie))
                    continue;

                var categoryIds = GetCategoryIds(movie);
                foreach (var catId in categoryIds)
                {
                    if (!deliveredByCategoryId.ContainsKey(catId))
                        deliveredByCategoryId[catId] = 0;

                    deliveredByCategoryId[catId]++;
                }
            }

            var copies = await _movieCopyRepository.GetAllAsync();
            var movieIdByCopyId = copies.ToDictionary(c => c.ID, c => c.MovieId);

            var damaged = await _damagedMovieRepository.GetAllAsync();
            var damagedInRange = damaged.Where(d => d.CreatedDate.Date >= startDate && d.CreatedDate.Date <= endDate).ToList();

            var damagedByCategoryId = new Dictionary<int, int>();

            foreach (var d in damagedInRange)
            {
                if (!movieIdByCopyId.TryGetValue(d.MovieCopyId, out int movieId))
                    continue;

                if (!movieById.TryGetValue(movieId, out var movie))
                    continue;

                var categoryIds = GetCategoryIds(movie);
                foreach (var catId in categoryIds)
                {
                    if (!damagedByCategoryId.ContainsKey(catId))
                        damagedByCategoryId[catId] = 0;

                    damagedByCategoryId[catId]++;
                }
            }

            const decimal ESTIMATED_REVENUE_PER_DELIVERY_ITEM = 0m;

            var result = new List<CategoryProfitReportDto>();

            foreach (var c in categories)
            {
                int deliveredCount = deliveredByCategoryId.TryGetValue(c.ID, out var del) ? del : 0;
                int damagedCount = damagedByCategoryId.TryGetValue(c.ID, out var dam) ? dam : 0;

                decimal revenue = deliveredCount * ESTIMATED_REVENUE_PER_DELIVERY_ITEM;
                decimal cost =
                    (damagedCount * ESTIMATED_DAMAGE_UNIT_COST) +
                    (deliveredCount * ESTIMATED_DELIVERY_UNIT_COST);

                result.Add(new CategoryProfitReportDto
                {
                    CategoryId = c.ID,
                    CategoryName = c.CategoryName,
                    DeliveredCount = deliveredCount,
                    DamagedCount = damagedCount,
                    TotalRevenue = revenue,
                    TotalCost = cost,
                    Profit = revenue - cost
                });
            }

            return result
                .OrderByDescending(x => x.DeliveredCount)
                .ToList();
        }

        private static int MonthsInclusive(DateTime from, DateTime to)
        {
            if (to < from) return 0;
            int fromMonths = (from.Year * 12) + from.Month;
            int toMonths = (to.Year * 12) + to.Month;
            return (toMonths - fromMonths) + 1;
        }

        private static int GetPrimaryCategoryId(Movie movie)
        {
            var catId = movie.MovieCategories?
                .Select(x => x.CategoryId)
                .FirstOrDefault() ?? 0;

            return catId;
        }

        private static List<int> GetCategoryIds(Movie movie)
        {
            return movie.MovieCategories?
                .Select(x => x.CategoryId)
                .Distinct()
                .ToList() ?? new List<int>();
        }
    }
}
