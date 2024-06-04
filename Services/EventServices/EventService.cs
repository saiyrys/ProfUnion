using DocumentFormat.OpenXml.Wordprocessing;
using Profunion.Dto.EventDto;
using Profunion.Interfaces.EventInterface;
using Profunion.Services.AdditionalServices;

namespace Profunion.Services.EventServices
{
    public class EventService : IEventService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventsRepository;
        private readonly Helpers _helper;


        public EventService(DataContext context, IMapper mapper, IEventRepository eventsRepository, Helpers helper)
        {
            _context = context;
            _mapper = mapper;
            _eventsRepository = eventsRepository;
            _helper = helper;
        }
        public async Task<(IEnumerable<GetEventDto> Events, int TotalPages)> GetEvents(int page, string search = null, string sort = null, string type = null)
        {
            int pageSize = 12;
            var events = await _eventsRepository.GetEvents();

            if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(sort) || !string.IsNullOrEmpty(type))
            {
                events = await _eventsRepository.SearchAndSortEvents(search, sort, type);
            }

            var eventsDto = _mapper.Map<List<GetEventDto>>(events);
            
            var pagination = await _helper.ApplyPaginations(eventsDto, page, pageSize);
            eventsDto = pagination.Item1;

            var totalPages = pagination.Item2;

            return (eventsDto, totalPages);
        }

        public async Task<GetEventDto> GetEventsByID(string eventId)
        {
            var events = await _eventsRepository.GetEvents();

            var currentEvent = events.Where(e => e.eventId == eventId).FirstOrDefault();

            if (currentEvent == null)
                throw new ArgumentException("Мероприятие не найдено");

            var eventDto = _mapper.Map<GetEventDto>(currentEvent);

            return (eventDto);
        }
        public async Task<bool> CreateEvents(CreateEventDto eventsCreate)
        {
            var eventsGet = await _eventsRepository.GetEvents();

            var existingEvents = eventsGet
                .FirstOrDefault(e => e.eventId.Trim().ToUpper() == eventsGet.Select(e => e.eventId.FirstOrDefault()));

            if (existingEvents != null)
                throw new ArgumentException("Такое обьявление уже создано");
            if (eventsCreate == null)
                throw new ArgumentException();

            var events = eventsGet
                .Where(e => e.title.Trim().ToUpper() == eventsCreate.title.ToUpper()
                && e.description.Trim().ToUpper() == eventsCreate.description.ToUpper())
                .FirstOrDefault();

            var eventsMap = _mapper.Map<Event>(eventsCreate);

            if (eventsCreate.categoriesId?.Any() == true)
            {
                eventsMap.EventCategories = eventsCreate.categoriesId
                    .Select(c => new EventCategories { CategoriesId = c, eventId = eventsMap.eventId })
                    .ToList();
            }


            if (eventsCreate.imagesId?.Any() == true)
            {
                eventsMap.EventUploads = eventsCreate.imagesId
                    .Select(c => new EventUploads { fileId = c, eventId = eventsMap.eventId })
                    .ToList();
            }

            if (!await _eventsRepository.CreateEvents(eventsMap))
            {
                throw new ArgumentException("Что то пошло не так при создании");
            }

            return true;

        }
        public async Task<bool> UpdateEvents(string eventId, UpdateEventDto updateEvent)
        {
            var currentEvent = await _eventsRepository.GetEventsByID(eventId);

            if (currentEvent == null)
                throw new ArgumentException("Ивет не существует");

            var eventType = typeof(Event);
            var updateEventType = typeof(UpdateEventDto);

            foreach (var property in updateEventType.GetProperties())
            {
                var newValue = property.GetValue(updateEvent);
                if (newValue != null)
                {
                    var eventProperty = eventType.GetProperty(property.Name);
                    if (eventProperty != null && eventProperty.CanWrite)
                    {
                        eventProperty.SetValue(currentEvent, newValue);
                    }
                }
            }

            currentEvent.updatedAt = DateTime.UtcNow;

            if (!await _eventsRepository.UpdateEvents(currentEvent))
            {
                throw new ArgumentException("Ошибка при обновлении события");
            }

            return true;
        }
        public async Task<bool> DeleteEvents(string eventId)
        {
            var eventToDelete = await _eventsRepository.GetEventsByID(eventId);

            if (eventToDelete == null)
                throw new ArgumentException();

            await _helper.CascadeDeletedEventContext(eventId);

            if (!await _eventsRepository.DeleteEvents(eventToDelete))
            {
                throw new ArgumentException("Ошибка удаления ивента");
            }

            return true;
        }
    }
}
