namespace Profunion.Services.AdditionalServices
{

    public class Helpers
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileRepository _fileRepository;
        private readonly IEventRepository _eventsRepository;

        public Helpers(DataContext context,
            IHttpContextAccessor httpContextAccessor,
            IEventRepository eventsRepository,
            IFileRepository fileRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _eventsRepository = eventsRepository;
            _fileRepository = fileRepository;
        }

        public async Task<Tuple<List<T>, int>> ApplyPaginations<T>(List<T> items, int page, int pageSize)
        {
            int totalItems = items.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            int skip = (page - 1) * pageSize;
            var itemsForPage = items.Skip(skip).Take(pageSize).ToList();

            return Tuple.Create(itemsForPage, totalPages);
        }

        public async Task<bool> CascadeDeletedEventContext(string eventId)
        {
            try
            {
                var applicationsToDelete = await _context.Application.Where(a => a.EventId == eventId).ToListAsync();
                _context.Application.RemoveRange(applicationsToDelete);

                var rejectedApplicationsToDelete = await _context.RejectedApplication.Where(ra => ra.EventId == eventId).ToListAsync();
                _context.RejectedApplication.RemoveRange(rejectedApplicationsToDelete);

                var reservationListsToDelete = await _context.ReservationList.Where(rl => rl.EventId == eventId).ToListAsync();
                _context.ReservationList.RemoveRange(reservationListsToDelete);

                var EventsCategoriesToDelete = await _context.EventCategories.Where(ec => ec.eventId == eventId).ToListAsync();
                _context.EventCategories.RemoveRange(EventsCategoriesToDelete);

                var eventUploads = await _context.EventUploads.Where(up => up.eventId == eventId).ToListAsync();

                foreach (var upload in eventUploads)
                {
                    var file = await _fileRepository.GetFile(upload.fileId);
                    if (file != null)
                    {
                        await _fileRepository.DeleteFile(file);
                    }
                }

                _context.EventUploads.RemoveRange(eventUploads);

                var eventUploadsToDelete = await _context.Uploads.OrderBy(up => up.fileName).FirstOrDefaultAsync();

                var uploadsToDelete = await _fileRepository.GetFile(eventUploadsToDelete.fileName);

                await _fileRepository.DeleteFile(uploadsToDelete);

                await _context.SaveChangesAsync();

                return true;
            }
            catch(Exception ex)
            {
                var fail = (ex.Message);
                return false; 
            }
        }

        public async Task<bool> CascadeDeletedCategoryContext(string categoryId)
        {
            try
            {
                var eventToDelete = await _context.Events.Include(e => e.EventCategories)
                .FirstOrDefaultAsync(e => e.EventCategories.Any(ec => ec.CategoriesId == categoryId));

                if (eventToDelete != null)
                {  
                    _context.Events.RemoveRange(eventToDelete);
                    await CascadeDeletedEventContext(eventToDelete.eventId);
                }

                return true;
            }
            catch (Exception ex)
            {
                var fail = (ex.Message);
                return false;
            }
        }

        public async Task<bool> CascadeDeletedUserContext(string userId)
        {
            try
            {
                var userToDelete = await _context.Users.Where(u => u.userId == userId).FirstOrDefaultAsync();

                if (userToDelete != null)
                {
                    var applicationsToDelete = await _context.Application.Where(a => a.UserId == userId).ToListAsync();
                    _context.Application.RemoveRange(applicationsToDelete);

                    var rejectedApplicationsToDelete = await _context.RejectedApplication.Where(ra => ra.UserId == userId).ToListAsync();
                    _context.RejectedApplication.RemoveRange(rejectedApplicationsToDelete);

                    var reservationListsToDelete = await _context.ReservationList.Where(rl => rl.UserId == userId).ToListAsync();
                    _context.ReservationList.RemoveRange(reservationListsToDelete);
                    _context.Users.RemoveRange(userToDelete);
                }

                return true;
            }
            catch (Exception ex)
            {
                var fail = (ex.Message);
                return false;
            }
        }
        public string VerifyByAccessToken()
        {
            string accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

            if (accessToken.StartsWith("Bearer"))
            {
                accessToken = accessToken.Substring("Bearer".Length).Trim();
            }

            else
            {
                return ("Некорректный формат токена доступа");
            }

            return accessToken;
        }

        


    }
}
