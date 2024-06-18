namespace Profunion.Services.FileServices
{
    public class FileService
    {
        public FileService()
        {
            
        }

        public async Task RemoveFiles()
        {
            foreach (var file in Directory.GetFiles("api/upload/"))
            {
                File.Delete(file);
            }
        }
    }
}
