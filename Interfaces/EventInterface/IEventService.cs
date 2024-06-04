namespace Profunion.Interfaces.EventInterface
{
    public interface IEventService
    {
        Task<(IEnumerable<GetEventDto> Events, int TotalPages)> GetEvents(int page, string searchString = null, string sort = null, string type = null);
/*        Task<ICollection<GetEventDto>> GetEventsWithCategory();*/
        Task<GetEventDto> GetEventsByID(string eventId);
      
        Task<bool> CreateEvents(CreateEventDto eventsCreate);
        Task<bool> UpdateEvents(string eventId,UpdateEventDto updateEvent);
        Task<bool> DeleteEvents(string eventId);
    }
}
