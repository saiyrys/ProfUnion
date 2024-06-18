namespace Profunion.Services.UserServices
{
    public class AuthentificationsService : IAuthentificationsService
    {
        private readonly IUserRepository _userRepository;
        private readonly HashingPassword _hashingPassword;
        private readonly IMapper _mapper;
        private readonly GenerateMultipleJWT _generateMJWT;
        private readonly Helpers _helper;
        public AuthentificationsService(IUserRepository userRepository, HashingPassword hashingPassword, IMapper mapper, GenerateMultipleJWT generateMJWT, Helpers helper)
        {
            _userRepository = userRepository;
            _hashingPassword = hashingPassword;
            _mapper = mapper;
            _generateMJWT = generateMJWT;
            _helper = helper;    
        }
        public async Task<bool> CreateUser(RegisterDto userCreate)
        {
            var users = await _userRepository.GetUsers();

            var existingUser = users.FirstOrDefault(u => u.userName.Trim().ToUpper() == userCreate.userName.ToUpper()
            || u.email.Trim().ToUpper() == userCreate.email.ToUpper()
            );

            if (existingUser != null)
                throw new ArgumentException("User Already Exists");

            var (hashedPassword, salt) = _hashingPassword.HashPassword(userCreate.password);

            var userMap = _mapper.Map<User>(userCreate);

            userMap.password = hashedPassword;
            userMap.salt = Convert.ToBase64String(salt);

            if (string.IsNullOrEmpty(userMap.role))
                userMap.role = "USER";

            if (!await _userRepository.CreateUser(userMap))
                throw new ArgumentException("Что то пошло не так при сохранении данных");

            return true;
        }
        public async Task<LoginResponseDto> LoginUser(LoginUserDto loginUser)
        {
            var users = await _userRepository.GetUsers();
            var user = users
                .Where(u => u.userName.Trim().ToUpper() == loginUser.userName.ToUpper()).FirstOrDefault();

            if (user == null)
                throw new ArgumentException("User not exists");
            if (!_hashingPassword.VerifyPassword(loginUser.password, user.password, user.salt))
            {
                throw new ArgumentException("Incorrect Password");
            }

            var accessToken = await _generateMJWT.GenerateAccessToken(user);
            var refreshToken = _generateMJWT.GenerateRefreshToken(user);

            var userDto = _mapper.Map<GetUserDto>(loginUser);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                User = userDto
            };

        }
        
        public async Task<LoginResponseDto> GetNewTokens(string refreshToken)
        {
            var user = await _userRepository.GetUserInfo(refreshToken);
            if (user == null)
            {
                throw new ArgumentException("User not exists");
            }

            var newAccessToken = _generateMJWT.GenerateAccessToken(user);

            var userDto = _mapper.Map<GetUserDto>(user);

            return new LoginResponseDto
            {
                AccessToken = await newAccessToken,
                User = userDto
            };

        }
    }
}
