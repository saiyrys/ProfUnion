namespace Profunion.Interfaces.UserInterface
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsers();
        Task<ICollection<User>> GetUsersByAdmin(string accessToken);
        Task<User> GetUserByID(string ID);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByRefreshToken(string refreshToken);
        Task<User> GetUserInfo(string accessToken);
        Task<ICollection<User>> SearchAndSortUsers(string search = null, string sort = null, string type = null);
        Task<bool> UserExists(string userID);
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<bool> logout();
        Task<bool> SaveUser();
    }
}
