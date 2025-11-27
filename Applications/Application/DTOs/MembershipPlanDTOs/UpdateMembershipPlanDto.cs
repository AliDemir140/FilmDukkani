namespace Application.DTOs.MembershipPlanDTOs
{
    public class UpdateMembershipPlanDto
    {
        public int Id { get; set; } // mecburi

        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public int MaxMoviesPerMonth { get; set; }
        public int MaxChangePerMonth { get; set; }
        public string? Description { get; set; }
    }
}
