namespace Profunion.Dto.ReservationDto
{
    public class UpdateReservationDto
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string eventId { get; set; }
        public int ticketsCount { get; set; }
    }
}
