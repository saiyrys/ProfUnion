namespace Profunion.Interfaces.CategoryInterface
{
    public interface ICategoriesService
    {
        Task<bool> CreateCategories(CreateCategoriesDto categoriesCreate);
        Task<bool> DeleteCategory(string categoryId);
    }
}
