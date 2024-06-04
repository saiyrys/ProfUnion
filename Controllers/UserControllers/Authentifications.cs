namespace Profunion.Controllers.UserControllers
{
    [Route("api/auth")]
    [ApiController]
    public class Authentifications : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly Helpers _helper;
        private readonly HashingPassword _hashingPassword;
        private readonly GenerateMultipleJWT _generateMJWT;

        public Authentifications(IConfiguration configuration,
            IUserRepository userRepository,
            IMapper mapper,
            Helpers helper,
            HashingPassword hashingPassword,
            GenerateMultipleJWT generateMJWT)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
            _helper = helper;
            _hashingPassword = hashingPassword;
            _generateMJWT = generateMJWT;
        }


        [HttpPost("register")]
/*        [Authorize(Roles = "ADMIN")]*/
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateUser(RegisterDto userCreate)
        {
            var users = await _userRepository.GetUsers();

            var existingUser = users
                .FirstOrDefault(u => u.userName.Trim().ToUpper() == userCreate.userName.ToUpper()
                );
            if (userCreate == null)
                return BadRequest(ModelState);

            if (existingUser != null)
            {
                ModelState.AddModelError(" ", "Пользователь с таким псевдонимом уже существует.");
                return StatusCode(422, ModelState);
            }

            var user = users
                .Where(u => u.userName.Trim().ToUpper() == userCreate.userName.ToUpper()
            )
            .FirstOrDefault();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (hashedPassword, salt) = _hashingPassword.HashPassword(userCreate.password);
            userCreate.password = hashedPassword;

            var userMap = _mapper.Map<User>(userCreate);

            userMap.password = hashedPassword;
            userMap.salt = Convert.ToBase64String(salt);

            if (string.IsNullOrEmpty(userMap.role))
            {
                userMap.role = "USER";
            }

            if (!await _userRepository.CreateUser(userMap))
            {
                ModelState.AddModelError("", "Что-то пошло не так при сохранении");
                return StatusCode(500, ModelState);
            }

            return Ok(true);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(204, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await _userRepository.GetUsers();

            var user = users
                .Where(u => u.userName.Trim().ToUpper() == loginUser.userName.ToUpper()
                )
                .FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError(" ", "Такого пользователя не существует");
                return StatusCode(422, ModelState);
            }

            if (string.IsNullOrEmpty(loginUser.password))
            {
                ModelState.AddModelError(" ", "Пароль не может быть пустым");
                return StatusCode(422, ModelState);
            }

            if (!_hashingPassword.VerifyPassword(loginUser.password, user.password, user.salt))
            {
                ModelState.AddModelError(" ", "Неверный пароль");
                return StatusCode(422, ModelState);
            }


            var accessToken = _generateMJWT.GenerateAccessToken(user);
            var refreshToken = _generateMJWT.GenerateRefreshToken(user);

            var userDto = _mapper.Map<UserDto>(loginUser);

            var response = new LoginResponseDto
            {
                accessToken = await accessToken,
                user = userDto 
            };

            return Ok(response);
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
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token is missing");
            }

            var user = await _userRepository.GetUserByRefreshToken(refreshToken);

            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }

            var newAccessToken = _generateMJWT.GenerateAccessToken(user);

            var userDto = _mapper.Map<UserDto>(user);

            var response = new LoginResponseDto
            {
                accessToken = await newAccessToken,
                user = userDto
            };

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
