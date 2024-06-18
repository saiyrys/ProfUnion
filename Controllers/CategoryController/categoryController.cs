namespace Profunion.Controllers.CategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class categoryController : Controller
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IMapper _mapper;
        private readonly ICategoriesService _categoriesService;
        public categoryController(
            ICategoriesRepository categoriesRepository,
            IMapper mapper,
            ICategoriesService categoriesService)
        {
            _categoriesRepository = categoriesRepository;
            _mapper = mapper;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        /*[Authorize]*/
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
            await _categoriesService.CreateCategories(categoriesCreate);
            return Ok("Категория успешно создана");
        }

        [HttpDelete("{categoryId}")]
/*        [Authorize(Roles = "ADMIN, MODER")]*/
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        { 
            await _categoriesService.DeleteCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }
    }
}
