using DocumentFormat.OpenXml.Office2010.Excel;

namespace Profunion.Services.FileServices
{
    public class FileRepository : IFileRepository
    {
        private readonly DataContext _context;

        public FileRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Uploads> GetFiles()
        {
            return await _context.Uploads.OrderBy(u => u.id).FirstOrDefaultAsync();
        }
        public async Task<string> GetFile(string fileName)
        {
            var upload = await _context.Uploads.FirstOrDefaultAsync(u => u.fileName == fileName);

            if (upload != null)
            {
                return upload.filePath;
            }

            return null;

        }
       /* public Task<Uploads> GetFileByName(string fileName)
        {
            throw new NotImplementedException();
        }*/
       
        public async Task<(string id, string url)> WriteFile<T>(IFormFile file)
        {
            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var extension = Path.GetExtension(file.FileName);
            var filename = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadDirectory, filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var uploadedFile = new Uploads
            {
                id = Guid.NewGuid().ToString(),
                fileName = filename,
                filePath = filePath
            };

            _context.Uploads.Add(uploadedFile);
            await _context.SaveChangesAsync();

            string baseUrl = "http://profunions.ru/api/upload/";
            string url = $"{baseUrl}{filename}";
            Uri uri = new Uri(url);

            return (uploadedFile.id,uri.ToString());
        }

        public async Task DeleteFile(string fileName)
        {
            string filePath = Path.Combine("http://profunions.ru/api/upload/", fileName);

            var fileToDelete = _context.Uploads.FirstOrDefault(u => u.filePath == filePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);

                _context.Uploads.Remove(fileToDelete);

                await _context.SaveChangesAsync();

            }
        }

        public async Task<bool> SaveUploads()
        {
            await _context.SaveChangesAsync();

            return true;
        }

       
    }
}
