namespace Profunion.Controllers.eventControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class eventController : Controller
    {
        private readonly IEventService _eventsService;
        public eventController(IEventService eventsService)
        {
            _eventsService = eventsService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetEvents(int page, string search = null, string sort = null, string type = null)
        {
            var (events, totalPages) = await _eventsService.GetEvents(page, search, sort, type);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(new {Items = events, TotalPages = totalPages});
        }

        [HttpGet("{eventId}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetEvent(string eventId)
        {
            var events = await _eventsService.GetEventsByID(eventId);

          

            return Ok(events);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEvents([FromBody] CreateEventDto eventsCreate)
        {
            if (eventsCreate == null)
            {
                return BadRequest();
            }

            var result = await _eventsService.CreateEvents(eventsCreate);

            if (!result)
            {
                return StatusCode(500, "Ошибка при создании мероприятия");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok("Мероприятие успешно создан");
        }

        [HttpPatch("{eventId}")]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEvents(string eventId, [FromBody] UpdateEventDto updateEvent)
        {
            if (updateEvent == null)
            {
                return BadRequest("Invalid data.");
            }

            var eventToUpdate = await _eventsService.UpdateEvents(eventId, updateEvent);

            if (eventToUpdate)
            {
                return NoContent(); 
            }
            else
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpDelete("{eventId}")]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEvents(string eventId)
        {
            var result = await _eventsService.DeleteEvents(eventId);

            if (!result)
            {
                return NotFound("Мероприятие не найдено");
            }

            return Ok("Мероприятие успешно удалено");
        }
    }
}
