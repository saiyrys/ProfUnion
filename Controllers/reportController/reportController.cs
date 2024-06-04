namespace Profunion.Controllers.reportController
{
    [ApiController]
    [Route("api/[controller]")]
    public class reportController : Controller
    {
        private readonly IReportRepository _report;
        public reportController(IReportRepository report)
        {
            _report = report;
        }

        [HttpPost("word/{eventId}")]
        public async Task<IActionResult> GenerateReport(string eventId)
        {
            try
            {
                await _report.GenerateReport(eventId);

                // Загрузка файла
                string filePath = "Отчёт.docx";
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = Path.GetFileName(filePath);

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка при генерации отчёта: {ex.Message}");
            }
        }
    }
}
