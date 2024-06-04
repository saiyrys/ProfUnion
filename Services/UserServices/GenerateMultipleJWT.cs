using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Profunion.Services.UserServices
{

    public class GenerateMultipleJWT
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenerateMultipleJWT(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> GenerateAccessToken(User user)
        {
            var tokenSettings = _configuration.GetSection("JwtOptions");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("S19v59LSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256); ;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                new Claim(ClaimTypes.Role, user.role)
            };

            var now = DateTime.UtcNow;
            var expires = now.AddHours(tokenSettings.GetValue<int>("ExpiresHours")); // Use the configured expiration time

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = now,
                Expires = expires,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Serialize the token using System.Text.Json
            var tokenJson = tokenHandler.WriteToken(token);

            /*var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Lax,
                Secure = true,
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("accessToken", tokenJson, cookieOptions);*/

            return await Task.FromResult(tokenJson);
        }


        public string GenerateRefreshToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Добавьте другие необходимые утверждения
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("S19v59LSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS")); // Замените на ваш секретный ключ
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Установите срок действия токена
                signingCredentials: creds);

            var refreshToken = new JwtSecurityTokenHandler().WriteToken(token);
            SetCookieRefresh(refreshToken);

            return refreshToken;
        }

        private void SetCookieRefresh(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Lax,
                Secure = true,
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }
}
