namespace Profunion.Controllers.ApplicationController
{
    [Route("api/[controller]")]
    [ApiController]
    public class applicationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IMapper _mapper;
        private readonly IRejectedApplicationRepository _rejectedAppRepository;
        private readonly IReservationList _reservationList;
        private readonly IEmailSender _emailSender;
        private readonly Helpers _helper;

        public applicationController(IApplicationRepository applicationRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IEventRepository eventRepository,
            IRejectedApplicationRepository rejectedAppRepository,
            IReservationList reservationList,
            IEmailSender emailSender,
            Helpers helper)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _applicationRepository = applicationRepository;
            _mapper = mapper;
            _rejectedAppRepository = rejectedAppRepository;
            _reservationList = reservationList;
            _emailSender = emailSender;
            _helper = helper;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplication(int page, string search = null, string sort = null, string type = null)
        {
            int pageSize = 12;

            var applications = await _applicationRepository.GetAllApplications();

            if (search != null || sort != null || type != null)
            {
                applications = await _applicationRepository.SearchAndSortApplications(search, sort, type);
            }

            var applicationsDto = _mapper.Map<List<GetApplicationDto>>(applications);

            foreach (var application in applicationsDto)
            {
                var user = await _userRepository.GetUserByID(application.userId);
                application.user = user;

                var events = await _eventRepository.GetEventsByID(application.eventId);
                application.events = events;
            }

            var pagination = await _helper.ApplyPaginations(applicationsDto, page, pageSize);
            applicationsDto = pagination.Item1;

            var totalPages = pagination.Item2;

            var result = new
            {
                Items = applicationsDto,
                TotalPages = totalPages,
            };

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDto createApplication)
        {
            var getApplication = await _applicationRepository.GetAllApplications();

            if (createApplication == null)
                return BadRequest(ModelState);

            var applications = getApplication
                .Where(a => a.UserId.Trim().ToUpper() == createApplication.userId.ToUpper()
                && a.EventId.Trim().ToUpper() == createApplication.eventId.ToUpper()
                ).FirstOrDefault();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookingMap = _mapper.Map<Application>(createApplication);

            if (bookingMap.status != "APPROVED" || bookingMap.status != "REJECTED")
                bookingMap.status = "PENDING";


            if (!await _applicationRepository.CreateApplication(bookingMap))
            {
                ModelState.AddModelError("", "Что-то пошло не так при сохранении");
                return StatusCode(500, ModelState);
            }


            await _emailSender.SendMessageAboutApplication(createApplication.userId, createApplication.eventId);

            return Ok("Заявка на мероприятие успешно создана");

        }

        [HttpPatch("{Id}")]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateApplication(string Id, [FromBody] JsonPatchDocument<UpdateApplicationDto> patchApp)
        {
            var application = await _applicationRepository.GetApplicationsByID(Id);
            if (application == null)
            {
                return NotFound();
            }

            var updatedApplication = _mapper.Map<UpdateApplicationDto>(application);
            patchApp.ApplyTo(updatedApplication, ModelState);

            _mapper.Map(updatedApplication, application);

            var success = await _applicationRepository.UpdateApplication(application);

            // Проверяем, если изменено свойство "status" на "Rejected".
            var statusProperty = patchApp.Operations.FirstOrDefault(op => op.path == "/status");
            if (statusProperty != null && statusProperty.value.ToString() == "REJECTED")
            {
                // Создаем объект RejectedApplication и копируем данные из Application.
                var rejectedApplication = new CreateRejectedApplicationDto
                {
                    eventId = application.EventId,
                    userId = application.UserId,
                    ticketsCount = application.ticketsCount,
                    createdAt = DateTime.Now
                };
                var createRejected = _mapper.Map<RejectedApplication>(rejectedApplication);
                // Вызываем метод репозитория для создания отклоненной заявки.

                await _emailSender.SendMessageAboutRejected(rejectedApplication.userId, rejectedApplication.eventId);
                var createRejectedResult = await _rejectedAppRepository.CreateRejected(createRejected);

                if (!createRejectedResult)
                {
                    // Если создание отклоненной заявки не удалось, вернуть ошибку.
                    return StatusCode(500, "Не удалось создать отклоненную заявку.");
                }
            }
            else if (statusProperty != null && statusProperty.value.ToString() == "APPROVED")
            {
                var currentEvent = await _eventRepository.GetEventsByID(application.EventId);

                if (currentEvent.totalTickets != 0 && currentEvent.totalTickets >= application.ticketsCount)
                {
                    currentEvent.totalTickets -= application.ticketsCount;

                    await _eventRepository.UpdateEvents(currentEvent);
                }
                else
                {
                    return StatusCode(500, "Не удалось принять заявку.");
                }

                // Создаем объект RejectedApplication и копируем данные из Application.
                var reservationList = new CreateReservationDto
                {
                    eventId = currentEvent.eventId,
                    userId = application.UserId,
                    ticketsCount = application.ticketsCount,
                    createdAt = DateTime.Now
                };

                var createReserv = _mapper.Map<ReservationList>(reservationList);
                // Вызываем метод репозитория для создания отклоненной заявки.

                await _emailSender.SendMessageAboutApply(reservationList.userId, reservationList.eventId);
                var createApprovedResult = await _reservationList.CreateReservation(createReserv);

                if (!createApprovedResult)
                {
                    // Если создание отклоненной заявки не удалось, вернуть ошибку.
                    return StatusCode(500, "Не удалось принять заявку.");
                }
            }

            if (!success)
            {
                // Возможно, здесь нужно обработать ситуацию неудачного обновления.
                return StatusCode(500);
            }

            return Ok(application);
        }

        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetUserApplication(string userId)
        {
            var user = await _userRepository.GetUserByID(userId);

            if (user == null)
                return NotFound("Пользователь в Базе данных не найден");

            var applications = _mapper.Map<List<GetApplicationDto>>(await _applicationRepository.GetAllApplications());
            var userApplications = applications.Where(app => app.userId == user.userId).ToList();

            if (userApplications.Any())
                return Ok(userApplications); 

            return NotFound("Для текущего пользователя заявок не найдено");

        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteApplication(string id)
        {
            var applicationToDelete = await _applicationRepository.GetApplicationsByID(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _applicationRepository.DeleteApplication(applicationToDelete))
            {
                ModelState.AddModelError(" ", "Ошибка удаления пользователя");
            }

            return NoContent();
        }

        [HttpGet("RejectedApplication")]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetRejectedApplication()
        {
            var rejectedApp = _mapper.Map<List<GetRejectedApplicationDto>>(await _rejectedAppRepository.GetAllRejectedApplication());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rejectedApp);

        }
    }
}
