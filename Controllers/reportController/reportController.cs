using Profunion.Services.ReportService;

namespace Profunion.Controllers.reportController
{
    [ApiController]
    [Route("api/[controller]")]
    public class reportController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly IReportRepository _report;
        private readonly ExcelReport _excelReport;
        public reportController(ExcelReport excelReport, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _excelReport = excelReport;
        }

        [HttpPost("excel/{eventId}")]
        public async Task<IActionResult> GenerateReport(string eventId)
        {
            try
            {
                byte[] reportBytes = await _excelReport.GenerateExcelReport(eventId);
                var currentEvent = await _eventRepository.GetEventsByID(eventId);

                string eventTitle = currentEvent.title;
               /* DateTime date = new DateTime*/
                // Загрузка файла
                string fileName = $"Отчёт по мероприятию {eventTitle} {DateTime.Now}.xlsx";

                return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка при генерации отчёта: {ex.Message}");
            }
        }
    }
}
