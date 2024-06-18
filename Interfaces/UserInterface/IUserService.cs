namespace Profunion.Interfaces.EventInterface
{
    public interface IUserService
    {
        Task<(IEnumerable<GetUserDto> User, int TotalPages)> GetUsersByAdmin(int page, string search = null, string sort = null, string type = null);
        Task<bool> UpdateUsers(string userId, UpdateUserDto updateUser);
        Task<bool> DeleteUser(string userId);
    }
}
