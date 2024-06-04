using System.IdentityModel.Tokens.Jwt;
using System.Data;
using System.Web;

namespace Profunion.Services.UserServices
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GenerateMultipleJWT _generateMJWT;
        public UserRepository(DataContext context, 
            IHttpContextAccessor httpContextAccessor, 
            IConfiguration configuration,
            GenerateMultipleJWT generateMJWT)
        {
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _generateMJWT = generateMJWT;
        }

        public async Task<User> GetUserByID(string ID)
        {
            return await _context.Users.Where(u => u.userId == ID).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users.Where(u => u.userName == username).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            string secretKey = "S19v59LSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS";

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(refreshToken);
            var claims = token.Claims;

            string userID = claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            User userInfo = new User
            {
                userId = userID
            };

            User userProfile = await GetUserByID(userID);

            return userProfile;
        }

        public async Task<User> GetUserInfo(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            string secretKey = "S19v59LSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS";

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadJwtToken(accessToken);

            var claims = token.Claims;

            string userID = claims.FirstOrDefault(x => x.Type == "nameid")?.Value;
          
            User userInfo = new User
            {
                userId = userID
            };

            User userProfile = await GetUserByID(userID);

            return userProfile;
        }

        public async Task<ICollection<User>> GetUsers()
        {
            return await _context.Users.OrderBy(u => u.userId).ToListAsync();
        }

        public async Task<ICollection<User>> GetUsersByAdmin(string accessToken)
        {
            var userInfo = await GetUserInfo(accessToken);

            var userRole = userInfo.role;

            if (userRole == "ADMIN")
                return await _context.Users.OrderBy(u => u.userId).ToListAsync();

            return null;

        }

        public async Task<ICollection<User>> SearchAndSortUsers(string search = null, string sort = null, string type = null)
        {
            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrEmpty(search))
            {
                search = HttpUtility.UrlDecode(search);
                search = search.ToLower();

                if (search.StartsWith("пользователь", StringComparison.OrdinalIgnoreCase))
                {
                    // Выполняем поиск пользователей с ролью "USER" в английском варианте
                    query = query.Where(u => u.role.ToLower() == "USER");
                }
                query = query.Where(u =>
                u.userName.ToLower().Contains(search) ||
                u.firstName.ToLower().Contains(search) ||
                u.middleName.ToLower().Contains(search) ||
                u.lastName.ToLower().Contains(search) ||
                u.role.ToLower().Contains(search) ||
                u.email.ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(type))
            {
                switch (sort.ToLower())
                {
                    case "alphabetic":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(u => u.userName);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(u => u.userName);
                        break;
                    case "role":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(u => u.role == "USER");
                        else if (type.ToLower() == "desc")
                            query = query.OrderBy(u => u.role == "ADMIN");
                        break;
                    case "createdAt":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(u => u.createdAt.Month).ThenBy(u => u.createdAt.Day);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(u => u.createdAt.Month).ThenBy(u => u.createdAt.Day); ;
                        break;
                }
            }

            return await query.ToListAsync();
        }

        public async Task<bool> UserExists(string userID)
        {
            return await _context.Users.AnyAsync(u => u.userId == userID);
        }

        public async Task<bool> CreateUser(User user)
        {
            user.userId = Guid.NewGuid().ToString();

            _context.Add(user);
            await _context.SaveChangesAsync();

            return await SaveUser();
        }

        public async Task<bool> UpdateUser(User user)
        {
            _context.Update(user);

            return await SaveUser();

        }

        public async Task<bool> DeleteUser(User user)
        {
            _context.Remove(user);

            return await SaveUser();
        }

        public async Task<bool> SaveUser()
        {
            await _context.SaveChangesAsync();

            return true;
        }

        public Task<bool> logout()
        {
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Task.FromResult(false);
            }
           
            return Task.FromResult(true);
        }

        
    }
}
