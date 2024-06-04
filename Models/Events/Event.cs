namespace Profunion.Models.Events
{
    public class Event
    {
        public Event()
        {
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
        }
        public string eventId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string organizer { get; set; }
        public DateTime eventDate { get; set; }
        public string link { get; set; }
        public int totalTickets { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }


        public virtual ICollection<Categories> Categories { get; set; }
        public virtual ICollection<EventCategories> EventCategories { get; set; }

        public virtual ICollection<Uploads> Uploads { get; set; }
        public virtual ICollection<EventUploads> EventUploads { get; set; }


    }
}
