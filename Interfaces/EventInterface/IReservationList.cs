namespace Profunion.Interfaces
{
    public interface IReservationList
    {
        Task<bool> CreateReservation(ReservationList reservation);
        Task<bool> UpdateReservation(ReservationList reservation);
        Task<bool> DeleteReservation(ReservationList reservation);
        Task<ICollection<ReservationList>> GetAllReservation();
        Task<ReservationList> GetReservationByID(string Id);
        Task<ReservationList> GetUserReservation(string UserId);
        Task<ICollection<ReservationList>> GetReservationByEvent(string eventId);
        Task<ICollection<ReservationList>> SearchAndSortReservation(string search = null, string sort = null, string type = null);
        Task<bool> SaveReservation();
    }
}
