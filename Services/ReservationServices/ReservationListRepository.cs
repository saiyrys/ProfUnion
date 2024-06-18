using Profunion.Interfaces.ReservationInterface;

namespace Profunion.Services.ReservationServices
{
    public class ReservationListRepository : IReservationList
    {
        private readonly DataContext _context;
        public ReservationListRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateReservation(ReservationList reservation)
        {
            reservation.Id = Guid.NewGuid().ToString();
            _context.Add(reservation);

            await _context.SaveChangesAsync();

            return await SaveReservation();
        }
        public async Task<bool> UpdateReservation(ReservationList reservation)
        {
            _context.Update(reservation);

            return await SaveReservation();
        }
        public async Task<bool> DeleteReservation(ReservationList reservation)
        {
            _context.Remove(reservation);

            return await SaveReservation();
        }

        public async Task<ICollection<ReservationList>> GetAllReservation()
        {
            return await _context.ReservationList.OrderBy(rl => rl.Id).ToListAsync();
        }

        public async Task<ReservationList> GetReservationByID(string Id)
        {
            return await _context.ReservationList.Where(rl => rl.Id == Id).FirstOrDefaultAsync();
        }
        public async Task<ICollection<ReservationList>> GetReservationByEvent(string eventId)
        {
            return await _context.ReservationList.Where(rl => rl.Event.eventId == eventId).ToListAsync();
        }
        public async Task<ReservationList> GetUserReservation(string UserID)
        {
            return await _context.ReservationList.Where(rl => rl.UserId == UserID).FirstOrDefaultAsync();
        }
        public async Task<ICollection<ReservationList>> SearchAndSortReservation(string search = null, string sort = null, string type = null)
        {
            IQueryable<ReservationList> query = _context.ReservationList;

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                query = query.Where(rl =>
                rl.User.userName.ToLower().Contains(search) ||
                rl.User.firstName.ToLower().Contains(search) ||
                rl.User.lastName.ToLower().Contains(search) ||
                rl.Event.title.ToLower().Contains(search) ||
                rl.Event.EventCategories.Select(ec => ec.Categories.name.ToLower()).Contains(search) ||
                rl.ticketsCount.ToString().Contains(search)
                );

            }

            if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(type))
            {
                switch (sort.ToLower())
                {
                    case "alphabetic":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(rl => rl.Event.title);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(rl => rl.Event.title);
                        break;
                    case "tickets":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(app => app.ticketsCount);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(app => app.ticketsCount);
                        break;
                    case "createdat":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(e => e.createdAt);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(e => e.createdAt);
                        break;
                    case "updatedat":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(e => e.updatedAt);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(e => e.updatedAt);
                        break;
                }
            }

            return await query.ToListAsync();
        }

        public async Task<bool> SaveReservation()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Логируем исключение с помощью Serilog
                Log.Error(ex, "Ошибка при сохранении изменений");
                return false;
            }
        }


    }
}
