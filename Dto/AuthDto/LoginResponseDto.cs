namespace Profunion.Dto.AuthDto
{
    public class LoginResponseDto
    {
        public string accessToken { get; set; }
        public UserDto.UserDto user { get; set; }
    }
}
