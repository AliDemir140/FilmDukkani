namespace Application.DTOs.AccountingReportDTOs
{
    public class DamagedMoviesReportItemDto
    {
        public int DamagedMovieId { get; set; }
        public int MovieCopyId { get; set; }
        public int MovieId { get; set; }
        public string? MovieTitle { get; set; }

        public bool IsSentToPurchase { get; set; }
        public string? Note { get; set; }
    }
}
