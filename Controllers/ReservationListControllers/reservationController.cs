using Profunion.Interfaces.ReservationInterface;

namespace Profunion.Controllers.ReservationListControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class reservationController : Controller
    {
        private readonly IReservationList _reservationList;
        private readonly IReservationService _reservationService;
        public reservationController(IReservationList reservationList,
            IReservationService reservationService)
        {
            _reservationList = reservationList;
            _reservationService = reservationService;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<ReservationList>>> GetReservation(int page, string search = null, string sort = null, string type = null)
        {
            var(reservations, totalPages) = await _reservationService.GetAllReservation(page, search, sort, type);
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new { Items = reservations, TotalPages = totalPages });
        }

        [HttpGet("{userId}")]
/*        [Authorize]*/
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetReservation(string userId)
        {

            var(userReservations, totaltickets) = await _reservationService.GetUserReservation(userId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new {Items = userReservations, TotalTickets = totaltickets });
        }

        [HttpPatch]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateReservation(string Id, UpdateReservationDto updateReservation)
        {
            await _reservationService.UpdateReservation(Id, updateReservation);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteReservation(string id)
        {
            var reservationToDelete = await _reservationList.GetReservationByID(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _reservationList.DeleteReservation(reservationToDelete))
            {
                ModelState.AddModelError(" ", "Ошибка удаления брони");
            }

            return NoContent();

        }
    }
}
