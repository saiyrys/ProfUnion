namespace Profunion.Services.BookingServices
{
    public class RejectedApplicationRepository : IRejectedApplicationRepository
    {
        private readonly DataContext _context;
        public RejectedApplicationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<RejectedApplication> GetRejectedByID(string ID)
        {
            return await _context.RejectedApplication.Where(b => b.Id == ID).FirstOrDefaultAsync();
        }

        public async Task<RejectedApplication> GetRejectedByUserID(string UserID)
        {
            return await _context.RejectedApplication.Where(b => b.UserId == UserID).FirstOrDefaultAsync();
        }
        
        public async Task<ICollection<RejectedApplication>> GetAllRejectedApplication()
        {
            return await _context.RejectedApplication.OrderBy(b => b.Id).ToListAsync();
        }

        public async Task<bool> CreateRejected(RejectedApplication rejectedApplication)
        {
            rejectedApplication.Id = Guid.NewGuid().ToString();
            _context.Add(rejectedApplication);

            await _context.SaveChangesAsync();

            return await Save();
        }

        public async Task<bool> Save()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Логируйте исключение, если необходимо
                Log.Error(ex, "Ошибка при сохранении изменений");
                return false;
            }
        }

    }
}
