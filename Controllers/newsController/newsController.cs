namespace Profunion.Controllers.newsController
{
    [ApiController]
    [Route("api/[controller]")]
    public class newsController : Controller
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;
        private readonly IFileRepository _fileRepository;
        private readonly Helpers _helper;
        private readonly DataContext _context;
        public newsController(INewsRepository newsRepository, IMapper mapper, Helpers helper, IFileRepository fileRepository, DataContext context)
        {
            _newsRepository = newsRepository;
            _fileRepository = fileRepository;
            _mapper = mapper;
            _helper = helper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<GetNewsDto>>> GetNewses(int page, string search = null, string sort = null, string type = null)
        {
            int pageSize = 12;

            var newses = await _newsRepository.GetNews();

            if (search != null || sort != null || type != null)
            {
                newses = await _newsRepository.SearchAndSort(search, sort, type);
            }

            var newsesDto = _mapper.Map<List<GetNewsDto>>(newses);

            var pagination = await _helper.ApplyPaginations(newsesDto, page, pageSize);

            newsesDto = pagination.Item1;


            var totalPage = pagination.Item2;

            var result = new
            {
                Items = newsesDto,
                TotalPages = totalPage
            };

            return Ok(result);
        }

        [HttpGet("{newsId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetNewsById(string newsId)
        {
            var newses = await _newsRepository.GetNews();

            var news = newses.Where(n => n.newsId == newsId).FirstOrDefault();

            if (news == null)
                return BadRequest("Новсть не существует");
            var newsDto = _mapper.Map<News>(news);
            newsDto.views += 1;

            await _newsRepository.UpdateNews(newsDto);

            return Ok(news);
        }

        [HttpPost]
/*        [Authorize(Roles = "Admin")]*/
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateNews(CreateNewsDto createNews)
        {
            var newsGet = await _newsRepository.GetNews();

            var existingNews = newsGet
                .FirstOrDefault(n => n.title.Trim().ToUpper() == newsGet.Select(n => n.title.FirstOrDefault()));

            if (existingNews != null)
            {
                ModelState.AddModelError(" ", "Такое обьявление уже создано");
            }

            var news = newsGet
              .Where(a => a.title.Trim().ToUpper() == createNews.title.ToUpper()
              && a.description.Trim().ToUpper() == createNews.description.ToUpper()
              ).FirstOrDefault();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newsMap = _mapper.Map<News>(createNews);

            if (createNews.imagesId?.Any() == true)
            {
                newsMap.NewsUploads = createNews.imagesId
                    .Select(u => new NewsUploads { fileId = u, newsId = createNews.newsId })
                    .ToList();
            }

            if (!await _newsRepository.CreateNews(newsMap))
            {
                ModelState.AddModelError(" ", "что то пошло не так при сохранении новости в базе данных");
                return StatusCode(500, ModelState);
            }

            return Ok("Новость успешно создан");

        }

      /*  [HttpPatch("{newsId}")]
        public async Task<IActionResult> UpdateEntity(string newsId, [FromBody] UpdateNewsDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }

            var entity = await _context.News.FindAsync(newsId);
            if (entity == null)
            {
                return NotFound();
            }

            // Обновите только те свойства, которые не равны null
            entity.Patch(updateDto);

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }*/


    }
}
