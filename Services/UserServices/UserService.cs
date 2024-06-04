namespace Profunion.Services.EventServices
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly Helpers _helper;


        public UserService(DataContext context, IMapper mapper, IUserRepository userRepository, Helpers helper)
        {
            _context = context;
            _mapper = mapper;
            _userRepository = userRepository;
            _helper = helper;
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

            currentUser.updatedAt = DateTime.UtcNow;

            if (!await _userRepository.UpdateUser(currentUser))
            {
                throw new ArgumentException("Ошибка при обновлении события");
            }

            return true;
        }
        
    }
}
