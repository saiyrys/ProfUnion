using DocumentFormat.OpenXml.Spreadsheet;
using Profunion.Models.UserModels;

namespace Profunion.Controllers.ApplicationController
{
    [Route("api/[controller]")]
    [ApiController]
    public class applicationController : Controller
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IRejectedApplicationRepository _rejectedAppRepository;
        private readonly IApplicationService _applicationService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public applicationController(IApplicationRepository applicationRepository,
            IRejectedApplicationRepository rejectedAppRepository,
            IApplicationService applicationService,
            IMapper mapper,
            IEmailSender emailSender)
        {

            _applicationRepository = applicationRepository;
            _rejectedAppRepository = rejectedAppRepository;
            _applicationService = applicationService;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplication(int page, string search = null, string sort = null, string type = null)
        {
            var (application, totalPages) = await _applicationService.GetApplication(page, search, sort, type);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new { Items = application, TotalPages = totalPages });
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDto createApplication)
        {

            await _applicationService.CreateApplication(createApplication);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (createApplication == null)
                return BadRequest();

            return Ok("Заявка на мероприятие успешно создана");

        }

        [HttpPatch("{Id}")]
        [Authorize(Roles = "ADMIN, MODER")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateApplication(string Id, UpdateApplicationDto updateApplication)
        {
            var application  = await _applicationService.UpdateApplication(Id, updateApplication);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(application);
        }

        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetUserApplication(string userId)
        {
            var userApp = await _applicationService.GetUserApplication(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(userApp);
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
