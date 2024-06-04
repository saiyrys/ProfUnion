namespace Profunion.Models.Events
{
    public class ReservationList
    {
        public ReservationList()
        {
            Id = Guid.NewGuid().ToString();
            updatedAt = DateTime.Now;
            createdAt = DateTime.Now;
        }
        public string Id { get; set;  }
        public string UserId { get; set; }
        public string EventId { get; set; }
        public int ticketsCount { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

    }
}
