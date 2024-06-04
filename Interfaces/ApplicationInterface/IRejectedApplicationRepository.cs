namespace Profunion.Interfaces.ApplicationInterface
{
    public interface IRejectedApplicationRepository
    {
        Task<ICollection<RejectedApplication>> GetAllRejectedApplication();
        Task<RejectedApplication> GetRejectedByID(string ID);
        Task<RejectedApplication> GetRejectedByUserID(string UserID);
        Task<bool> CreateRejected (RejectedApplication rejectedApplication);
        Task<bool> Save();
    }
}
