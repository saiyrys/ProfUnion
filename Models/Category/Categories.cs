namespace Profunion.Models.Category
{
    public class Categories
    {
        public string Id { get; set; }
        public string name { get; set; }

        public string color { get; set; }

        public virtual ICollection<EventCategories> EventCategories { get; set; }
    }
}
