namespace Profunion.Interfaces.ReportInterface
{
    public interface IReportRepository
    {
        Task GenerateReport(string eventId);
    }
}
