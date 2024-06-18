namespace Profunion.Interfaces.UserInterface
{
    public interface IAuthentificationsService
    {
        Task<bool> CreateUser(RegisterDto userCreate);
        Task<LoginResponseDto> LoginUser(LoginUserDto loginUser);
        Task<LoginResponseDto> GetNewTokens(string refreshToken);

    }
}
