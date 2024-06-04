namespace Profunion.Interfaces.EventInterface
{
    public interface IEventRepository
    {
        Task<ICollection<GetEventDto>> GetEvents();
/*        Task<ICollection<GetEventDto>> GetEventsWithCategory();*/
        Task<Event> GetEventsByID(string eventID);
        Task<ICollection<GetEventDto>> SearchAndSortEvents(string searchString = null, string sort = null, string type = null);
        Task<bool> EventsExists(string titles);
        Task<bool> CreateEvents(Event events);
        Task<bool> UpdateEvents(Event events);
        Task<bool> DeleteEvents(Event events);
        Task<bool> SaveEvents();
    }
}
