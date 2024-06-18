namespace Profunion.Dto.AuthDto
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public GetUserDto User { get; set; }
    }
}
