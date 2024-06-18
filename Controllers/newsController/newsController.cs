using Microsoft.Extensions.Logging;

namespace Profunion.Controllers.newsController
{
    [ApiController]
    [Route("api/[controller]")]
    public class newsController : Controller
    {
        private readonly INewsService _newsService;
        public newsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<GetNewsDto>>> GetNewses(int page, string search = null, string sort = null, string type = null)
        {
            var (newses, totalPages) = await _newsService.GetNewses(page, search, sort, type);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(new { Items = newses, TotalPages = totalPages });
        }

        [HttpGet("{newsId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetNewsById(string newsId)
        {
            var news = await _newsService.GetNews(newsId);

            return Ok(news);
        }

        [HttpPost]
/*        [Authorize(Roles = "Admin")]*/
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateNews(CreateNewsDto createNews)
        {
            if (createNews == null)
            {
                return BadRequest();
            }

            var result = await _newsService.CreateNews(createNews);

            if (!result)
            {
                return StatusCode(500, "Ошибка при создании мероприятия");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok("Новость успешно создан");

        }

        [HttpPatch("{newsId}")]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateNews(string newsId, [FromBody] UpdateNewsDto updateNews)
        {

            if (updateNews == null)
            {
                return BadRequest("Invalid data.");
            }

            var newsToUpdate = await _newsService.UpdateNews(newsId, updateNews);

            if (newsToUpdate)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpDelete("{newsId}")]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEvents(string newsId)
        {
            var result = await _newsService.DeleteNews(newsId);

            if (!result)
            {
                return NotFound("Новость не найдено");
            }

            return Ok("Новость успешно удалена");
        }


    }
}
