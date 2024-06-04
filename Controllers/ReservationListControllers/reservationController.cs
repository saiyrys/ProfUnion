namespace Profunion.Controllers.ReservationListControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class reservationController : Controller
    {
        private readonly IReservationList _reservationList;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly Helpers _helper;
        public reservationController(IReservationList reservationList,
            IUserRepository userRepository,
            IEventRepository eventRepository,
            IMapper mapper,
            Helpers helper)
        {
            _reservationList = reservationList;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
            _helper = helper;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<ReservationList>>> GetReservation(int page, string search = null, string sort = null, string type = null)
        {
            int pageSize = 12;

            var reservations = await _reservationList.GetAllReservation();

            if (search != null || sort != null || type != null)
            {
                reservations = await _reservationList.SearchAndSortReservation(search, sort, type);
            }

            var reservationsDto = _mapper.Map<List<GetReservationDto>>(reservations);

            foreach (var reservation in reservationsDto)
            {
                var user = await _userRepository.GetUserByID(reservation.userId);
                reservation.user = _mapper.Map<GetUserDto>(user);

                var events = await _eventRepository.GetEvents();
                var currentEvents = events.Where(e => e.eventId == reservation.eventId).FirstOrDefault();

                reservation.events = currentEvents;
            }

            var pagination = await _helper.ApplyPaginations(reservationsDto, page, pageSize);
            reservationsDto = pagination.Item1;

            var totalPages = pagination.Item2;

            var result = new
            {
                Items = reservationsDto,
                TotalPages = totalPages,
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(result);
        }

        [HttpGet("{userId}")]
/*        [Authorize]*/
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetReservation(string userId)
        {

            var reservations = _mapper.Map<List<GetReservationDto>>(await _reservationList.GetAllReservation());

            // Фильтруем резервации только для указанного пользователя
            var userReservations = reservations.Where(r => r.userId == userId).ToList();

            var eventsWithCategories = await _eventRepository.GetEvents();

            foreach (var reservation in reservations)
            {
                var events = await _eventRepository.GetEvents();
                var currentEvents = events.Where(e => e.eventId == reservation.eventId).FirstOrDefault();

                reservation.events = currentEvents;
            }

            var result = new
            {
                Items = reservations,
                ticketsCount = userReservations.Select(r => r.ticketsCount)
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(result);
        }

        /* [HttpPatch]
         [ProducesResponseType(204)]
         public async Task<IActionResult> UpdateReservation(string Id, [FromBody] JsonPatchDocument<UpdateReservationDto> patchApp)
         {
             string accessToken = _verify.VerifyUserByAccessToken();

             var reservationList = await _reservationList.GetReservationByID(Id);
             if (reservationList == null)
             {
                 return NotFound();
             }
             var updatedReservationList = _mapper.Map<UpdateReservationDto>(reservationList);

             patchApp.ApplyTo(updatedReservationList, ModelState);

             var success = await _reservationList.UpdateReservation(reservationList);

             if (!success)
             {
                 return StatusCode(500);
             }

             return Ok(reservationList);
         }*/

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
