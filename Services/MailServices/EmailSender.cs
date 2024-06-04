using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace Profunion.Services.MailServices
{
    public class EmailSender : IEmailSender
    {
        private readonly string _sendGridApiKey;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly DataContext _context;
        public EmailSender(IOptions<SendGridClientOptions> options,
            IUserRepository userRepository,
            IEventRepository eventRepository,
            DataContext context)
        {
            _sendGridApiKey = options.Value.ApiKey;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _context = context;
        }

        public async Task SendEmail(string userId, string subject, string message)
        {
            var user = await _userRepository.GetUserByID(userId);

            var client = new SendGridClient(_sendGridApiKey);

            var msg = new SendGridMessage
            {
                From = new EmailAddress(user.email),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(user.email));

            await client.SendEmailAsync(msg);
        }

        public async Task SendMessageAboutApplication(string userId, string eventId)
        {
            var user = await _userRepository.GetUserByID(userId);

            var events = await _eventRepository.GetEventsByID(eventId);

            MailMessage message = new MailMessage
            {
                From = new MailAddress("profunion.kst@mail.ru", "Администратор сайта"),
                Subject = "Заявка на мероприятие",
                Body = $"Вами была подана заявка на участие в мероприятии '{events.title}'",
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(user.email));

            SmtpClient smtp = new SmtpClient("smtp.mail.ru")
            {
                Credentials = new NetworkCredential("profunion.kst@mail.ru", "G6fdPeZfDHefV5wT3xu9"),
                EnableSsl = true,
                Timeout = 10000 // 10 секунд
            };
            await smtp.SendMailAsync(message);
        }
        public async Task SendMessageAboutApply(string userId, string eventId)
        {
            var user = await _userRepository.GetUserByID(userId);

            var events = await _eventRepository.GetEventsByID(eventId);

            MailMessage message = new MailMessage
            {
                From = new MailAddress("profunion.kst@mail.ru", "Администратор сайта"),
                Subject = "Заявка на мероприятие",
                Body = $"Ваша заявка была принята'{events.title}' подробнее см. на сайте",
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(user.email));

            SmtpClient smtp = new SmtpClient("smtp.mail.ru")
            {
                Credentials = new NetworkCredential("profunion.kst@mail.ru", "G6fdPeZfDHefV5wT3xu9"),
                EnableSsl = true,
                Timeout = 10000 // 10 секунд
            };
            await smtp.SendMailAsync(message);
        }
        public async Task SendMessageAboutRejected(string userId, string eventId)
        {
            var user = await _userRepository.GetUserByID(userId);

            var events = await _eventRepository.GetEventsByID(eventId);

            MailMessage message = new MailMessage
            {
                From = new MailAddress("profunion.kst@mail.ru", "Администратор сайта"),
                Subject = "Заявка на мероприятие",
                Body = $"Ваша заявка на'{events.title}' была отклонена подробнее см. на сайте",
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(user.email));

            SmtpClient smtp = new SmtpClient("smtp.mail.ru")
            {
                Credentials = new NetworkCredential("profunion.kst@mail.ru", "G6fdPeZfDHefV5wT3xu9"),
                EnableSsl = true,
                Timeout = 10000 // 10 секунд
            };
            await smtp.SendMailAsync(message);
        }
    }
}
