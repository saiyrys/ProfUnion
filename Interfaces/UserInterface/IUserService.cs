namespace Profunion.Interfaces.EventInterface
{
    public interface IUserService
    {
/*        Task<(IEnumerable<GetEventDto> Events, int TotalPages)> GetEvents(int page, string searchString = null, string sort = null, string type = null);
*//*        Task<ICollection<GetEventDto>> GetEventsWithCategory();*//*
        Task<GetEventDto> GetEventsByID(string eventId);
      
        Task<bool> CreateEvents(CreateEventDto eventsCreate);*/
        Task<bool> UpdateUsers(string userId,UpdateUserDto updateUser);
/*        Task<bool> DeleteEvents(string eventId);*/
    }
}
