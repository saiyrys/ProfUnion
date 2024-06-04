namespace Profunion.Interfaces.ApplicationInterface
{
    public interface IApplicationRepository
    {
        Task<ICollection<Application>> GetAllApplications();
        Task<Application> GetApplicationsByID(string ID);
        Task<Application> GetUserApplications(string UserID);
        Task<ICollection<Application>> SearchAndSortApplications(string search = null, string sort = null, string type = null);
        Task<bool> CreateApplication(Application application);
        Task<bool> UpdateApplication(Application application);
        Task<bool> DeleteApplication(Application application);
        Task<bool> SaveApplication();
    }
}
