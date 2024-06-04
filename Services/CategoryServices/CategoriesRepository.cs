namespace Profunion.Services.CategoryServices
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly DataContext _context;

        public CategoriesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateCategories(Categories categories)
        {
            categories.Id = Guid.NewGuid().ToString();

            _context.Add(categories);

            await _context.SaveChangesAsync();

            return await SaveCategories();
        }

        public async Task<bool> DeleteCategories(Categories categories)
        {

            _context.Remove(categories);

            return await SaveCategories();
        }

        public async Task<ICollection<Categories>> GetCategories()
        {
            return await _context.Categories.OrderBy(c => c.Id).ToListAsync();
        }

        public async Task<Categories> GetCategoriesByID(string id)
        {
            return await _context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Categories> GetEventsByName(string Name)
        {
            return await _context.Categories.Where(c => c.name == Name).FirstOrDefaultAsync();

        }

        public async Task<bool> SaveCategories()
        {
            var saved = await _context.SaveChangesAsync();

            return saved > 0 ? true : false;
        }

        public Task<bool> UpdateCategories(Categories categories)
        {
            throw new NotImplementedException();
        }
    }
}
