namespace Profunion.Interfaces.UserInterface
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsers();
        Task<User> GetUserByID(string ID);
        Task<User> GetUserInfo(string accessToken);
        Task<ICollection<User>> SearchAndSortUsers(string search = null, string sort = null, string type = null);
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<bool> SaveUser();
    }
}
