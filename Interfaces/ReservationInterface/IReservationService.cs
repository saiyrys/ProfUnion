namespace Profunion.Interfaces.ReservationInterface
{
    public interface IReservationService
    {
        Task<(IEnumerable<GetReservationDto> Reservation, int TotalPage)> GetAllReservation(int page, string search = null, string sort = null, string type = null);

        Task<(IEnumerable<GetReservationDto> reservations, int totalTickets)> GetUserReservation(string userId);

        Task<UpdateReservationDto> UpdateReservation(string Id, UpdateReservationDto updateReservation);
    }
}
