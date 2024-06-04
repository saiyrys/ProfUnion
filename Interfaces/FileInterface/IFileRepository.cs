namespace Profunion.Interfaces.FileInterface
{
    public interface IFileRepository
    {
        Task<Uploads> GetFiles();
        Task<string> GetFile(string fileName);
/*        Task<Uploads> GetFileByName(string fileName);*/
        Task<(string id, string url)> WriteFile<T>(IFormFile files);
        Task DeleteFile(string filePath);

    }
}
