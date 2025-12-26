namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }

        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
    }
}
