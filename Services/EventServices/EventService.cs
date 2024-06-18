using Profunion.Interfaces.CacheInterface;
namespace Profunion.Services.EventServices
{
    public class EventService : IEventService
    {
        private readonly DataContext _context;
        private readonly ICacheProvider _cacheProvider;
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventsRepository;
        private readonly Helpers _helper;
        private readonly CascadeDeleteMethods _cascade;


        public EventService(DataContext context, IMapper mapper, IEventRepository eventsRepository, Helpers helper, ICacheProvider cacheProvider, IFileRepository fileRepository, CascadeDeleteMethods cascade)
        {
            _context = context;
            _mapper = mapper;
            _eventsRepository = eventsRepository;
            _fileRepository = fileRepository;
            _helper = helper;
            _cacheProvider = cacheProvider;
            _cascade = cascade;
        }
        public async Task<(IEnumerable<GetEventDto> Events, int TotalPages)> GetEvents(int page, string search = null, string sort = null, string type = null)
        {
            int pageSize = 12;
            var cacheKey = $"GetEvents_{page}_{search}_{sort}_{type}";

            var cachedData = _cacheProvider.Get<(IEnumerable<GetEventDto>, int)>(cacheKey);
            if (cachedData.Item1 != null && cachedData.Item2 != 0)
            {
                return cachedData;
            }

            var events = await _eventsRepository.GetEvents();

            if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(sort) || !string.IsNullOrEmpty(type))
            {
                events = await _eventsRepository.SearchAndSortEvents(search, sort, type);
            }

            var eventsDto = _mapper.Map<List<GetEventDto>>(events);
            
            var pagination = await _helper.ApplyPaginations(eventsDto, page, pageSize);
            eventsDto = pagination.Item1;

            var totalPages = pagination.Item2;

            _cacheProvider.Set(cacheKey, (eventsDto, totalPages), TimeSpan.FromMinutes(10));

            return (eventsDto, totalPages);
        }

        public async Task<GetEventDto> GetEventsByID(string eventId)
        {
            var cacheKey = $"GetEventsByID_{eventId}";
            var cachedEvent = _cacheProvider.Get<GetEventDto>(cacheKey);
            if (cachedEvent != null)
            {
                return cachedEvent;
            }

            var events = await _eventsRepository.GetEvents();

            var currentEvent = events.Where(e => e.eventId == eventId).FirstOrDefault();

            if (currentEvent == null)
                throw new ArgumentException("Мероприятие не найдено");

            var eventDto = _mapper.Map<GetEventDto>(currentEvent);

            _cacheProvider.Set(cacheKey, eventDto, TimeSpan.FromMinutes(10));

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
            await _helper.UpdateEntity<Event, UpdateEventDto>(eventId, updateEvent);

            if (updateEvent.categoriesId != null && updateEvent.categoriesId.Any())
            {
                var eventCategories = _context.EventCategories.Where(ec => ec.eventId == eventId).ToList();
                _context.EventCategories.RemoveRange(eventCategories);

                foreach (var categoryId in updateEvent.categoriesId)
                {
                    _context.EventCategories.Add(new EventCategories
                    {
                        eventId = eventId,
                        CategoriesId = categoryId
                    });
                }
            }

            if (updateEvent.imagesId != null && updateEvent.imagesId.Any())
            {
                foreach (var uploadId in updateEvent.imagesId)
                {
                   
                    var eventUpload = await _context.EventUploads.FirstOrDefaultAsync(up => up.eventId == eventId && up.fileId == uploadId);
                    if (eventUpload != null)
                    {
                        _context.EventUploads.Remove(eventUpload);
                        await _fileRepository.DeleteFile(eventId,uploadId);
                    }
                }
                _context.EventUploads.RemoveRange(_context.EventUploads.Where(up => up.eventId == eventId));
                foreach (var uploadId in updateEvent.imagesId)
                {
                    _context.EventUploads.Add(new EventUploads
                    {
                        eventId = eventId,
                        fileId = uploadId
                    });
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteEvents(string eventId)
        {
            var eventToDelete = await _eventsRepository.GetEventsByID(eventId);

            if (eventToDelete == null)
                throw new ArgumentException();

            await _cascade.CascadeDeletedEventContext(eventId);

            if (!await _eventsRepository.DeleteEvents(eventToDelete))
            {
                throw new ArgumentException("Ошибка удаления ивента");
            }

            return true;
        }
    }
}
