namespace Profunion.Services.EventServices
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly HashingPassword _hashingPassword;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly Helpers _helper;
        private readonly CascadeDeleteMethods _cascade;


        public UserService(DataContext context, IMapper mapper, IUserRepository userRepository, Helpers helper, IHttpContextAccessor httpContext, HashingPassword hashingPassword, CascadeDeleteMethods cascade)
        {
            _context = context;
            _hashingPassword = hashingPassword;
            _mapper = mapper;
            _userRepository = userRepository;
            _helper = helper;
            _httpContext = httpContext;
            _cascade = cascade;
        }
        public async Task<(IEnumerable<GetUserDto> User, int TotalPages)> GetUsersByAdmin(int page, string search = null, string sort = null, string type = null)
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

            
            return (userDto, totalPages);
        }
       

        public async Task<bool> UpdateUsers(string userId, UpdateUserDto updateUser)
        {
            var currentUser = await _userRepository.GetUserByID(userId);

            if (currentUser == null)
                throw new ArgumentException("Ивет не существует");

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
                throw new ArgumentException("Ошибка при обновлении события");
            }

            return true;
        }

        public async Task<bool> DeleteUser(string userId)
        {

            var userToDelete = await _userRepository.GetUserByID(userId);

            
            await _cascade.CascadeDeletedUserContext(userId);

            if (!await _userRepository.DeleteUser(userToDelete))
            {
                throw new ArgumentException(" ", "Ошибка удаления пользователя");
            }

            return true;
        }

    }
}
