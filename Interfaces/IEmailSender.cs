namespace Profunion.Interfaces
{
    public interface IEmailSender
    {
        Task SendMessageAboutApplication(string userId, string eventId);
        Task SendMessageAboutApply(string userId, string eventId);
        Task SendMessageAboutRejected(string userId, string eventId);
    }
}
