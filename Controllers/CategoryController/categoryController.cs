namespace Profunion.Controllers.CategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class categoryController : Controller
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly Helpers _helper;

        public categoryController(
            ICategoriesRepository categoriesRepository,
            IMapper mapper,
            IUserRepository userRepository,
            Helpers helper)
        {
            _categoriesRepository = categoriesRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _helper = helper;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Categories>))]
        public async Task<IActionResult> GetCategories()
        {
            var categories = _mapper.Map<List<CategoriesDto>>(await _categoriesRepository.GetCategories());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCategories([FromBody] CreateCategoriesDto categoriesCreate)
        {
            var categoriesGet = await _categoriesRepository.GetCategories();

            var existingcategories = categoriesGet
                .FirstOrDefault(c => c.name.Trim().ToUpper() == categoriesCreate.name.ToUpper());

            if (existingcategories != null)
            {
                ModelState.AddModelError(" ", "Такая категория уже существует");
            }

            if (categoriesCreate == null)
                return BadRequest(ModelState);

            var categories = categoriesGet
                .Where(c => c.name.Trim().ToUpper() == categoriesCreate.name.ToUpper()
                && c.color.Trim().ToLower() == categoriesCreate.color.ToLower()
                ).FirstOrDefault();

            /*if (string.IsNullOrEmpty(categories.color))
            {
                categories.color = "default";
            }*/

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriesMap = _mapper.Map<Categories>(categoriesCreate);

            if (await _categoriesRepository.CreateCategories(categoriesMap))
            {
                ModelState.AddModelError("", "Что-то пошло не так при сохранении");
                return StatusCode(500, ModelState);
            }

            return Ok("Категория успешно создана");
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        { 
            var categoryToDelete = await _categoriesRepository.GetCategoriesByID(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _helper.CascadeDeletedCategoryContext(categoryId);
            
            if (!await _categoriesRepository.DeleteCategories(categoryToDelete))
            {
                ModelState.AddModelError(" ", "Ошибка удаления категории");
            }

            return NoContent();
        }
    }
}
