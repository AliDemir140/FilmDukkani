using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountingController : ControllerBase
    {
        private readonly AccountingServiceManager _accountingService;

        public AccountingController(AccountingServiceManager accountingService)
        {
            _accountingService = accountingService;
        }

        // GET: api/accounting/profit-loss
        [HttpGet("profit-loss")]
        public async Task<IActionResult> GetProfitLoss(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate == default || endDate == default)
                return BadRequest("startDate ve endDate zorunludur.");

            if (startDate > endDate)
                return BadRequest("startDate endDate'den büyük olamaz.");

            var result = await _accountingService
                .GetProfitLossSummaryAsync(startDate, endDate);

            return Ok(result);
        }

        // GET: api/accounting/member-report
        [HttpGet("member-report")]
        public async Task<IActionResult> GetMemberReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _accountingService
                .GetMemberProfitReportAsync(startDate, endDate);

            return Ok(result);
        }

        // GET: api/accounting/movie-report
        [HttpGet("movie-report")]
        public async Task<IActionResult> GetMovieReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _accountingService
                .GetMovieProfitReportAsync(startDate, endDate);

            return Ok(result);
        }

        // GET: api/accounting/category-report
        [HttpGet("category-report")]
        public async Task<IActionResult> GetCategoryReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _accountingService
                .GetCategoryProfitReportAsync(startDate, endDate);

            return Ok(result);
        }
    }
}
