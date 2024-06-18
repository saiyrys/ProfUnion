namespace Profunion.Interfaces.ApplicationInterface
{
    public interface IApplicationService
    {
        Task<(IEnumerable<GetApplicationDto>, int TotalPages)> GetApplication(int page, string search = null, string sort = null, string type = null);

        Task<bool> CreateApplication(CreateApplicationDto createApplication);
        Task<bool> UpdateApplication(string Id, UpdateApplicationDto updateApplication);

        Task<List<GetUserApplicationDto>> GetUserApplication(string userId);
    }
}
