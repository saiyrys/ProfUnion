namespace Profunion.Models.uploadsModels
{
    public class EventUploads
    {
        public string eventId { get; set; }
        public Event Event { get; set; }

        public string fileId { get; set; }
        public Uploads Uploads { get; set; }
    }
}
