namespace Profunion.Interfaces.CategoryInterface
{
    public interface ICategoriesRepository
    {
        Task<ICollection<Categories>> GetCategories();
        Task<Categories> GetCategoriesByID(string id);
        Task<Categories> GetEventsByName(string Name);
        Task<bool> CreateCategories(Categories categories);
        Task<bool> UpdateCategories(Categories categories);
        Task<bool> DeleteCategories(Categories categories);
        Task<bool> SaveCategories();
    }
}
