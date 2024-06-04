namespace Profunion.Dto.ReservationDto
{
    public class GetUserReservationDto
    {
        public string id { get; set; }
        public string userId { get; set; }
        public GetUserDto user { get; set; }
        public List<GetEventDto> events { get; set; }
        public string eventId { get; set; }
        public int ticketsCount { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
