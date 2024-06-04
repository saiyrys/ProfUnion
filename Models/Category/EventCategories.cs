namespace Profunion.Models.Category
{
    public class EventCategories
    {
        public string CategoriesId { get; set; }
        public Categories Categories { get; set; }

        public string eventId { get; set; }
        public Event Event { get; set; }

    }
}
