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
                string eventTitle = currentEvent.First().title;
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"Отчёт по мероприятию {eventTitle}");

                worksheet.Column(1).Width = 18.57;
                worksheet.Column(2).Width = 18.57;
                worksheet.Column(3).Width = 18.57;
                worksheet.Column(4).Width = 18.57;
                worksheet.Column(5).Width = 18.57;

                worksheet.Cells[1, 1].Value = "Организатор";
                worksheet.Cells[1, 2].Value = "Дата проведения";
                worksheet.Cells[1, 3].Value = "Мероприятие";
                worksheet.Cells[1, 4].Value = "Категории";
                worksheet.Cells[1, 5].Value = "Кол-во участников";


                for (int i = 0; i < currentEvent.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = currentEvent[i].organizer; // Организатор
                    worksheet.Cells[2 + i, 2].Value = currentEvent[i].eventDate; // Дата проведения
                    worksheet.Cells[2 + i, 3].Value = currentEvent[i].title; // Мероприятие
                    worksheet.Cells[2 + i, 4].Value = string.Join(", ", currentEvent[i].categories.Select(c => c.name)); // Категории
                    worksheet.Cells[2 + i, 5].Value = membersOfEvent.Count(); // Кол-во участников
                }

                return package.GetAsByteArray();

            }
        }
    }
}
