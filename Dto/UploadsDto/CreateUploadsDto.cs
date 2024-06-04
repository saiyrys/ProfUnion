namespace Profunion.Dto.UploadsDto
{
    public class CreateUploadsDto
    {
        public int id { get; }
        public IFormFile files { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }
    }
}
