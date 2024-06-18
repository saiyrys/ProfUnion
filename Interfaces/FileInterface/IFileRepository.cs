namespace Profunion.Interfaces.FileInterface
{
    public interface IFileRepository
    {
        Task<Uploads> GetFiles();
        Task<string> GetFile(string fileId);
/*        Task<Uploads> GetFileByName(string fileName);*/
        Task<(string id, string url)> WriteFile<T>(IFormFile files);
        Task DeleteFile(string eventId, string fileId);

    }
}
