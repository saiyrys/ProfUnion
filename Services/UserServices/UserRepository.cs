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
        public async Task<User> GetUserInfo(string Token)
        {
            if (string.IsNullOrEmpty(Token))
            {
                throw new ArgumentNullException(nameof(Token));
            }

            string secretKey = "S19v59LSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS";

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadJwtToken(Token);

            var claims = token.Claims;

            string userId = claims.FirstOrDefault(x => x.Type == "nameid" || x.Type == "sub")?.Value;
          
            User userInfo = new User
            {
                userId = userId
            };

            User userProfile = await GetUserByID(userId);

            return userProfile;
        }

        public async Task<ICollection<User>> GetUsers()
        {
            return await _context.Users.OrderBy(u => u.userId).ToListAsync();
        }

        public async Task<ICollection<User>> SearchAndSortUsers(string search = null, string sort = null, string type = null)
        {
            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

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
                    case "createdat":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(u => u.createdAt);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(u => u.createdAt);
                        break;
                    case "updatedat":
                        if (type.ToLower() == "asc")
                            query = query.OrderBy(u => u.updatedAt);
                        else if (type.ToLower() == "desc")
                            query = query.OrderByDescending(u => u.updatedAt);
                        break;
                }
            }

            return await query.ToListAsync();
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
        
    }
}
