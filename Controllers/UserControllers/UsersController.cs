using Microsoft.Extensions.Logging;
using Profunion.Models.Events;
using Profunion.Services.UserServices;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Profunion.Controllers.UserControllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly Helpers _helper;

        public UsersController(IUserRepository userRepository,
            IUserService userService,
            Helpers helper)
            {
            _userRepository = userRepository;
            _userService = userService;
            _helper = helper;
            }

        [HttpGet()]
        [Authorize(Roles = "MODER, ADMIN")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetUsersByAdmin(int page, string search= null, string sort = null, string type = null)
        {
            var (users, totalPages) = await _userService.GetUsersByAdmin(page, search, sort, type);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(new { Items = users, TotalPages = totalPages });

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
                role = userProfile.role,
                password = userProfile.password
            };

            return Ok(profile);
        }

        [HttpPatch("{userId}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUser)
        {
            if (updateUser == null)
            {
                return BadRequest("Пользователь не найден");
            }

            var userToUpdate = await _userService.UpdateUsers(userId, updateUser);
            if (userToUpdate)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("{userId}")]
        [Authorize (Roles = "ADMIN")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userService.DeleteUser(userId);

            if (!result)
            {
                return NotFound("Пользователь не найден");
            }

            return Ok("Пользователь удалён");
        }
    }
}
