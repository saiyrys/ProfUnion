using Microsoft.Extensions.Logging;
using Profunion.Models.uploadsModels;

namespace Profunion.Services.AdditionalServices
{
    public class CascadeDeleteMethods
    {
        private readonly DataContext _context;
        private readonly IFileRepository _fileRepository;
        public CascadeDeleteMethods(DataContext context, IFileRepository fileRepository)
        {
            _context = context;
            _fileRepository = fileRepository;
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
                    // Используем метод DeleteFile для удаления файла
                    await _fileRepository.DeleteFile(eventId, upload.fileId);
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                var fail = (ex.Message);
                return false;
            }
        }
        public async Task<bool> CascadeDeletedNewsContext(string newsId)
        {
            try
            {
                var newsUploads = await _context.NewsUploads.Where(up => up.newsId == newsId).ToListAsync();
                foreach (var upload in newsUploads)
                {
                    await _fileRepository.DeleteFile(newsId, upload.fileId);
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                var fail = (ex.Message);
                return false;
            }
        }
        public async Task<bool> CascadeDeletedCategoryContext(string categoryId)
        {
            try
            {
                var eventsToDelete = await _context.Events.Include(e => e.EventCategories)
                .Where(e => e.EventCategories.Any(ec => ec.CategoriesId == categoryId)).ToListAsync();

                foreach (var eventToDelete in eventsToDelete)
                {
                    var countCategories = eventToDelete.EventCategories.Count;

                    if (countCategories == 1)
                    {
                        _context.Events.RemoveRange(eventToDelete);
                        await CascadeDeletedEventContext(eventToDelete.eventId);
                    }
                    else
                    {
                        var EventsCategoriesToDelete = await _context.EventCategories.Where(ec => ec.CategoriesId == categoryId).ToListAsync();
                        _context.EventCategories.RemoveRange(EventsCategoriesToDelete);
                    } 
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
    }
}
