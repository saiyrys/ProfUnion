using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using Profunion.Interfaces.ReservationInterface;

namespace Profunion.Services.ReportService
{
    public class ExcelReport
    {
        private readonly IEventRepository _eventRepository;
        private readonly IReservationList _reservationList;
        public ExcelReport(IEventRepository eventRepository, IReservationList reservationList)
        {
            _eventRepository = eventRepository;
            _reservationList = reservationList;
        }
        public async Task<byte[]> GenerateExcelReport(string eventId)
        {
            var events = await _eventRepository.GetEvents();
            var currentEvent = events.Where(e => e.eventId == eventId).ToList();
            var membersOfEvent = await _reservationList.GetReservationByEvent(eventId);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"Отчёт по мероприятию {currentEvent.Select(e => e.title)}");

                worksheet.Column(1).Width = 18.57;
                worksheet.Column(2).Width = 18.57;

                worksheet.Cells[1, 1].Value = "Организатор";
                worksheet.Cells[3, 1].Value = "Дата проведения";
                worksheet.Cells[5, 1].Value = "Мероприятие";
                worksheet.Cells[7, 1].Value = "Категории";
                worksheet.Cells[9, 1].Value = "Кол-во участников";

                
                for(int i = 0; i < currentEvent.Count; i++)
                {
                    worksheet.Cells[1, 2 + i].Value = currentEvent[i].organizer; // Организатор
                    worksheet.Cells[3, 2 + i].Value = currentEvent[i].eventDate; // Дата проведения
                    worksheet.Cells[5, 2 + i].Value = currentEvent[i].title; // Мероприятие
                    worksheet.Cells[7, 2 + i].Value = string.Join(", ", currentEvent[i].categories.Select(c => c.name)); // Категории
                    worksheet.Cells[9, 2 + i].Value = membersOfEvent.Count();
                }

                return package.GetAsByteArray();

            }
        }
    }
}
