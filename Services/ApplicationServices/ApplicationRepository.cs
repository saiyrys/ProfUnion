namespace Profunion.Services.BookingServices
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly DataContext _context;
        public ApplicationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Application> GetApplicationsByID(string ID)
        {
            return await _context.Application.Where(b => b.Id == ID).FirstOrDefaultAsync();
        }

        public async Task<Application> GetUserApplications(string UserID)
        {
            return await _context.Application.Where(b => b.UserId == UserID).FirstOrDefaultAsync();
        }
        
        public async Task<ICollection<Application>> GetAllApplications()
        {
            return await _context.Application.OrderBy(b => b.Id).ToListAsync();
        }
        public async Task<ICollection<Application>> SearchAndSortApplications(string search = null, string sort = null, string type = null)
        {
            IQueryable<Application> query = _context.Application;

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                query = query.Where(app =>
                app.User.userName.ToLower().Contains(search) ||
                app.User.firstName.ToLower().Contains(search) ||
                app.User.lastName.ToLower().Contains(search) ||
                app.User.firstName.ToLower().Contains(search) ||
                app.Event.title.ToLower().Contains(search) ||
                app.Event.EventCategories.Select(ec => ec.Categories.name.ToLower()).Contains(search) ||
                app.ticketsCount.ToString().ToLower().Contains(search));
            }

            if(!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(type))
            {
                switch (sort.ToLower())
                {
                    case "alphabetic":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(app => app.Event.title);
                        /*                                .ThenBy(app => app.User.userName);*/
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(app => app.Event.title);
/*                                .ThenBy(app => app.Event.description);*/
                        break;
                    case "createdAt":
                         if (type.ToLower() == "asc")
                            query = query.OrderBy(e => e.createdAt.Day);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(e => e.createdAt.Day);
                        break;
                    case "ticketsCount":
                        if (type.ToLower() == "inc")
                            query = query.OrderBy(app => app.ticketsCount);
                        else if (type.ToLower() == "dec")
                            query = query.OrderBy(app => app.ticketsCount);
                        break;

                }
            }
            return query.ToList();

        }
        public async Task<bool> CreateApplication(Application application)
        {
            _context.Add(application);
            await _context.SaveChangesAsync();

            return await SaveApplication();
        }
        public async Task<bool> UpdateApplication(Application application)
        {
            _context.Update(application);
            return await SaveApplication();

        }
        public async Task<bool> SaveApplication()
        {
           
                await _context.SaveChangesAsync();
                return true;

        }

        public async Task<bool> DeleteApplication(Application application)
        {
            _context.Remove(application);

            return await SaveApplication();
        }

        
    }
}
