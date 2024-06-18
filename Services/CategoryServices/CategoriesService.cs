using Profunion.Interfaces.CategoryInterface;

namespace Profunion.Services.CategoriesServices
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IMapper _mapper;
        private readonly CascadeDeleteMethods _cascade;
        public CategoriesService(ICategoriesRepository categoriesRepository, IMapper mapper, CascadeDeleteMethods cascade)
        {
            _categoriesRepository = categoriesRepository;
            _mapper = mapper;
            _cascade = cascade;
        }
        public async Task<bool> CreateCategories(CreateCategoriesDto categoriesCreate)
        {
            var categoriesGet = await _categoriesRepository.GetCategories();

            var existingcategories = categoriesGet
                .FirstOrDefault(c => c.name.Trim().ToUpper() == categoriesCreate.name.ToUpper());

            if (existingcategories != null)
            {
                throw new ArgumentException();
            }

            if (categoriesCreate == null)
                throw new ArgumentException();

            var categories = categoriesGet
                .Where(c => c.name.Trim().ToUpper() == categoriesCreate.name.ToUpper()
                && c.color.Trim().ToLower() == categoriesCreate.color.ToLower()
                ).FirstOrDefault();

            var categoriesMap = _mapper.Map<Categories>(categoriesCreate);

            if (await _categoriesRepository.CreateCategories(categoriesMap))
            {
                throw new ArgumentException("", "Что-то пошло не так при сохранении");
            }

            return true;
        }

        public async Task<bool> DeleteCategory(string categoryId)
        {
            var categoryToDelete = await _categoriesRepository.GetCategoriesByID(categoryId);

            await _cascade.CascadeDeletedCategoryContext(categoryId);

            if (!await _categoriesRepository.DeleteCategories(categoryToDelete))
            {
                throw new ArgumentException(" ", "Ошибка удаления категории");
            }

            return true;
        }
    }
}
