using Microsoft.Extensions.Logging;
using Profunion.Services.UserServices;

namespace Profunion.Controllers.UserControllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly Helpers _helper;
        private readonly HashingPassword _hashingPassword;
        private readonly DataContext _context;

        public UsersController(IUserRepository userRepository,
            HashingPassword hashingPassword,
            IMapper mapper,
            DataContext context,
            Helpers helper)
            {
            _userRepository = userRepository;
            _mapper = mapper;
            _hashingPassword = hashingPassword;
            _helper = helper;
            _context = context;
            }

        [HttpGet()]
        [Authorize(Roles = "MODER, ADMIN")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByAdmin(int page, string search= null, string sort = null, string type = null)
        {
            int pageSize = 12;
            var users = await _userRepository.GetUsers();

            if (search != null || sort != null || type != null)
            {
                users = await _userRepository.SearchAndSortUsers(search, sort, type);
            }

            var userDto = _mapper.Map<List<GetUserDto>>(users);

            var pagination = await _helper.ApplyPaginations(userDto, page, pageSize);
            userDto = pagination.Item1;

            var totalPages = pagination.Item2;

            var result = new
            {
                Items = userDto,
                TotalPages = totalPages,
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(result);
        }

        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProfile()
        {
            string accessToken = HttpContext.Request.Headers["accessToken"];

            accessToken = _helper.VerifyByAccessToken();

            var userProfile = await _userRepository.GetUserInfo(accessToken);


            if (userProfile == null)
            {
                return NotFound("Профиль пользователя не найден");
            }
           
            var profile = new User
            {
                userId = userProfile.userId,
                userName = userProfile.userName,
                firstName = userProfile.firstName,
                lastName = userProfile.lastName,
                middleName = userProfile.middleName,
                role = userProfile.role
            };

            return Ok(profile);
        }

        [HttpGet("{userId}")]
        [Authorize (Roles = "ADMIN")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser(string userId)
        {
            var userProfile = await _userRepository.GetUserByID(userId);

            if (userProfile == null)
            {
                return NotFound("Профиль пользователя не найден");
            }

            var profile = new User
            {
                userId = userProfile.userId,
                userName = userProfile.userName,
                firstName = userProfile.firstName,
                lastName = userProfile.lastName,
                middleName = userProfile.middleName,
                email = userProfile.email,
                role = userProfile.role
            };

            return Ok(profile);
        }

        [HttpPatch("{userId}")]
        [Authorize (Roles = "ADMIN")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUser)
        {
            var currentUser = await _userRepository.GetUserByID(userId);

            if (currentUser == null)
                return NotFound("Пользователь не существует");

            var userType = typeof(User);
            var updateUserType = typeof(UpdateUserDto);

            foreach (var property in updateUserType.GetProperties())
            {
                var newValue = property.GetValue(updateUser);
                if (newValue != null)
                {
                    var userProperty = userType.GetProperty(property.Name);
                    if (userProperty != null && userProperty.CanWrite)
                    {
                        userProperty.SetValue(currentUser, newValue);
                    }
                }
            }

            var (hashedPassword, salt) = _hashingPassword.HashPassword(currentUser.password);
            currentUser.password = hashedPassword;

            var userMap = _mapper.Map<User>(currentUser);

            userMap.password = hashedPassword;
            userMap.salt = Convert.ToBase64String(salt);

            currentUser.updatedAt = DateTime.UtcNow;

            if (!await _userRepository.UpdateUser(currentUser))
            {
                return BadRequest("Ошибка обновления");
            }

            return NoContent();
        }

        [HttpDelete("{userId}")]
        [Authorize (Roles = "ADMIN")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            
            var userToDelete = await _userRepository.GetUserByID(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _helper.CascadeDeletedUserContext(userId);

            if (!await _userRepository.DeleteUser(userToDelete))
            {
                ModelState.AddModelError(" ", "Ошибка удаления пользователя");
            }
            return NoContent();
        }
    }
}
