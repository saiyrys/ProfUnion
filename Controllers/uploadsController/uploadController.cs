namespace Profunion.Controllers.uploadsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class uploadController : Controller
    {

        private readonly IFileRepository _fileRepository;
        public uploadController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }


        [HttpGet("{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetImage(string fileName)
        {
            var filePath = await _fileRepository.GetFile(fileName);

            if (filePath != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
               
                return File(fileStream, "image/png"); 
            }
            else
            {
                return BadRequest("File not found");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(IFormFile image, CancellationToken cancellationtoken)
        {
            (string id, string url) = await _fileRepository.WriteFile<string>(image);

            var result = new
            {
                Id = id,
                Url = url,
            };

            return Ok(result);
        }

        [HttpDelete("{fileName}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string fileName)
        {
            var fileInfo = await _fileRepository.GetFile(fileName);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           /* if (!*/
            /*await _fileRepository.DeleteFile(fileInfo);*//*)*//*
            {
                ModelState.AddModelError(" ", "Ошибка удаления файла");
            }*/

            return NoContent();
        }

    }
}
