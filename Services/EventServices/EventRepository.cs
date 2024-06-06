using DocumentFormat.OpenXml.Vml;
using System.Globalization;

namespace Profunion.Services.EventServices
{
    public class EventRepository : IEventRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;


        public EventRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ICollection<GetEventDto>> GetEvents()
        {
            string baseUrl = "https://profunions.ru/api/upload/";

            var events = await _context.Events
                .Include(e => e.EventCategories)
                    .ThenInclude(ec => ec.Categories)
                .Include(e => e.EventUploads)
                    .ThenInclude(eu => eu.Uploads)
                 .Select(e => new GetEventDto
                 {
                     eventId = e.eventId,
                     title = e.title,
                     description = e.description,
                     organizer = e.organizer,
                     eventDate = e.eventDate.ToString("yyyy-MM-dd HH:mm"),
                     link = e.link,
                     totalTickets = e.totalTickets,
                     createdAt = e.createdAt.ToString("yyyy-MM-dd"),
                     updatedAt = e.updatedAt.ToString("yyyy-MM-dd"),
                     images = e.EventUploads.Select(eu => new GetUploadsDto
                     {
                         id = eu.fileId,
                         Url = $"{baseUrl}{_context.Uploads.FirstOrDefault(u => u.id == eu.fileId).fileName}"
                     }).ToList(),

                     categories = e.EventCategories.Select(ec => new Categories { Id = ec.Categories.Id, name = ec.Categories.name }).ToList()
                 }).ToListAsync();

            return events;
        }

        public async Task<Event> GetEventsByID(string eventID)
        {
            return await _context.Events.Where(a => a.eventId == eventID).FirstOrDefaultAsync();
        }
       
        public async Task<ICollection<GetEventDto>> SearchAndSortEvents(string searchString = null, string sort = null, string type = null)
        {
            var events = await GetEvents();

            IQueryable<GetEventDto> query = events.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower(); // Преобразуем строку поиска в нижний регистр для удобства сравнения

                query = query.Where(e =>
                     e.title.ToLower().Contains(searchString) ||
                     e.description.ToLower().Contains(searchString) ||
                     e.categories.Any(c => c.name.ToString().ToLower().Contains(searchString)) ||
                     e.totalTickets.ToString().Contains(searchString) ||
                     e.createdAt.ToString().Contains(searchString) ||
                     e.updatedAt.ToString().Contains(searchString));
            }
            if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(type))
            {
                switch (sort.ToLower())
                {
                    case "alphabetic":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(e => e.title);
                        /*.ThenBy(e => e.description);*/
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(e => e.title);
                                /*.ThenBy(e => e.description);*/
                        break;
                    case "eventDate":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(e => DateTime.Parse(e.eventDate).Day);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(e => DateTime.Parse(e.eventDate).Day);
                        break;
                    case "tickets":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(e => e.totalTickets);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(e => e.totalTickets);
                        break;
                }
            }
            return query.ToList();
        }

        public async Task<bool> EventsExists(string titles)
        {
            return await _context.Events.AnyAsync(a => a.title == titles);
        }

        public async Task<bool> CreateEvents(Event events)
        {
            events.eventId = Guid.NewGuid().ToString();

            _context.Add(events);

            await _context.SaveChangesAsync();

            return await SaveEvents();
        }


        public async Task<bool> SaveEvents()
        {
              await _context.SaveChangesAsync();
              
            return true;
        }

        public async Task<bool> UpdateEvents(Event events)
        {
            _context.Update(events);
            return await SaveEvents();
        }
        public async Task<bool> DeleteEvents(Event events)
        {
            _context.Remove(events);

            return await SaveEvents();
        }
    }
}
