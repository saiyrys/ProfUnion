namespace Profunion.Controllers.UserControllers
{
    [Route("api/auth")]
    [ApiController]
    public class Authentifications : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthentificationsService _authService;
        private readonly Helpers _helper;

        public Authentifications(
            IUserRepository userRepository,
            IAuthentificationsService authService,
            Helpers helper)
        {
            _userRepository = userRepository;
            _authService = authService;
            _helper = helper;
        }


        [HttpPost("register")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateUser(RegisterDto userCreate)
        {
            await _authService.CreateUser(userCreate);
             
            if (userCreate == null)
                return BadRequest(ModelState);

          
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            return Ok(true);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(204, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> LoginUser(LoginUserDto loginUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(loginUser.password))
            {
                ModelState.AddModelError(" ", "Пароль не может быть пустым");
                return StatusCode(422, ModelState);
            }

            var login = await _authService.LoginUser(loginUser);

            return Ok(login);
        }

        [HttpGet("access-token")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUsersByAccessToken()
        {
            var accessToken = _helper.VerifyByAccessToken();

            var userAttributes = await _userRepository.GetUserInfo(accessToken);

            var role = new ReturnRoleDto
            {
                role = userAttributes.role
            };

            return Ok(role);
        }

        [HttpPost("login/access-token")]
        [ProducesResponseType(200, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetNewTokens()
        {
            string refreshToken = HttpContext.Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token is missing");
            }

            var response = await _authService.GetNewTokens(refreshToken);

            return Ok(response);
        }


        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> logout()
        {
            Response.Cookies.Delete("refreshToken");

            return Ok(true); 
        }
    }
}
