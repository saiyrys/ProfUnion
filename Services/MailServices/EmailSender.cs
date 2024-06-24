using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using static Microsoft.IO.RecyclableMemoryStreamManager;

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


        public async Task SendMessageAboutApplication(string userId, string eventId)
        {
            var user = await _userRepository.GetUserByID(userId);
            var events = await _eventRepository.GetEventsByID(eventId);

            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo-profsouz-kst.png");
            LinkedResource logo = new LinkedResource(logoPath, MediaTypeNames.Image.Jpeg)
            {
                ContentId = "logo"
            };

            string htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; text-align: center;'>
                    <table width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: #f9f9f9; padding: 20px;'>
                        <tr>
                            <td align='center'>
                                <table width='600' cellspacing='0' cellpadding='0' border='0' style='background-color: #ffffff; border: 1px solid #eaeaea;'>
                                    <tr>
                                        <td style='padding: 20px; text-align: center;'>
                                            <img src='cid:logo' alt='Профсоюз КСТ' style='width: 100px; height: auto;'>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 20px; text-align: center;'>
                                            <h2 style='color: #333333;'>Заявка на мероприятие</h2>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 20px;'>
                                            <p style='color: #555555;'>
                                                Вами была подана заявка на участие в мероприятии '{events.title}'.<br>
                                                Мы рассмотрим её в течение 3 рабочих дней и пришлем уведомление о вынесеном решении.
                                            </p>
                                            <p style='color: #555555;'>
                                                Следить за своими заявками и управлять ими можно через наш сайт:
                                            </p>
                                            <p style='text-align: center;'>
                                                <a href='https://profunions.ru/' target='_blank'>
                                                    Профсоюз КСТ
                                                </a>
                                            </p>
                                        </td>
                                    </tr>
                                    <tr>
                                         <td style='padding: 20px; text-align: center; background-color: #f1f1f1;'>
                                            <p style='color: #777777; font-size: 12px;'>Следите за новостями и мероприятиями на нашем сайте</p>
                                                <p style='text-align: center;'/>
                                                <a href='https://profunions.ru/' target='_blank'> Профсоюз КСТ </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            htmlView.LinkedResources.Add(logo);

            var message = new MailMessage
            {
                From = new MailAddress("profunion.kst@mail.ru", "Администратор сайта"),
                Subject = "Заявка на мероприятие",
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(user.email));
            message.AlternateViews.Add(htmlView);

            using (var smtp = new SmtpClient("smtp.mail.ru", 587))
            {
                smtp.Credentials = new NetworkCredential("profunion.kst@mail.ru", "QQ2ds83aRy3iMNvnSdqY");
               /* smtp.EnableSsl = true;*/
                smtp.Timeout = 10000; // 10 секунд


                await smtp.SendMailAsync(message);

            }
            
        }
        public async Task SendMessageAboutApply(string userId, string eventId)
        {
            var user = await _userRepository.GetUserByID(userId);

            var events = await _eventRepository.GetEventsByID(eventId);

            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo-profsouz-kst.png");
            LinkedResource logo = new LinkedResource(logoPath, MediaTypeNames.Image.Jpeg)
            {
                ContentId = "logo"
            };

            string htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; text-align: center;'>
                    <table width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: #f9f9f9; padding: 20px;'>
                        <tr>
                            <td align='center'>
                                <table width='600' cellspacing='0' cellpadding='0' border='0' style='background-color: #ffffff; border: 1px solid #eaeaea;'>
                                    <tr>
                                        <td style='padding: 20px; text-align: center;'>
                                            <img src='cid:logo' alt='Профсоюз КСТ' style='width: 100px; height: auto;'>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 20px; text-align: center;'>
                                            <h2 style='color: #333333;'>Заявка на мероприятие</h2>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 20px;'>
                                            <p style='color: #555555;'>
                                                Вами была подана заявка на участие в мероприятии '{events.title}'. По
                                                данной заявке было принято положительное решение.<br>                                                
                                            </p>
                                            <p style='color: #555555;'>
                                                Подробнее смотрите на нашем сайте в личном кабинете:
                                            </p>
                                            <p style='text-align: center;'>
                                                <a href='https://profunions.ru/' target='_blank'>
                                                    Профсоюз КСТ
                                                </a>
                                            </p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 20px; text-align: center; background-color: #f1f1f1;'>
                                            <p style='color: #777777; font-size: 12px;'>Следите за новостями и мероприятиями на нашем сайте</p>
                                                <p style='text-align: center;'/>
                                                <a href='https://profunions.ru/' target='_blank'> Профсоюз КСТ </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            htmlView.LinkedResources.Add(logo);

            var message = new MailMessage
            {
                From = new MailAddress("profunion.kst@mail.ru", "Администратор сайта"),
                Subject = "Заявка на мероприятие",
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(user.email));
            message.AlternateViews.Add(htmlView);

            using (var smtp = new SmtpClient("smtp.mail.ru", 587))
            {
                smtp.Credentials = new NetworkCredential("profunion.kst@mail.ru", "QQ2ds83aRy3iMNvnSdqY");
               /* smtp.EnableSsl = true;*/
                smtp.Timeout = 10000; // 10 секунд


                await smtp.SendMailAsync(message);

            }
        }
        public async Task SendMessageAboutRejected(string userId, string eventId)
        {
            var user = await _userRepository.GetUserByID(userId);

            var events = await _eventRepository.GetEventsByID(eventId);

            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo-profsouz-kst.png");
            LinkedResource logo = new LinkedResource(logoPath, MediaTypeNames.Image.Jpeg)
            {
                ContentId = "logo"
            };

            string htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; text-align: center;'>
                    <table width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: #f9f9f9; padding: 20px;'>
                        <tr>
                            <td align='center'>
                                <table width='600' cellspacing='0' cellpadding='0' border='0' style='background-color: #ffffff; border: 1px solid #eaeaea;'>
                                    <tr>
                                        <td style='padding: 20px; text-align: center;'>
                                            <img src='cid:logo' alt='Профсоюз КСТ' style='width: 100px; height: auto;'>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 20px; text-align: center;'>
                                            <h2 style='color: #333333;'>Заявка на мероприятие</h2>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 20px;'>
                                            <p style='color: #555555;'>
                                                К сожалению, ваша заявка на участие в событии '{events.title}' была отклонена. 
                                                Мы предлагаем вам рассмотреть другие мероприятия, доступные на нашем сайте.
                                            </p>
                                            <p style='color: #555555;'>
                                                Перейти на страницу с мероприятиями:
                                            </p>
                                            <p style='text-align: center;'>
                                                <a href='https://profunions.ru/events' target='_blank'>
                                                    Профсоюз КСТ
                                                </a>
                                            </p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 20px; text-align: center; background-color: #f1f1f1;'>
                                            <p style='color: #777777; font-size: 12px;'>Следите за новостями и мероприятиями на нашем сайте</p>
                                                <p style='text-align: center;'/>
                                                <a href='https://profunions.ru/' target='_blank'> Профсоюз КСТ </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            htmlView.LinkedResources.Add(logo);

            var message = new MailMessage
            {
                From = new MailAddress("profunion.kst@mail.ru", "Администратор сайта"),
                Subject = "Заявка на мероприятие",
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(user.email));
            message.AlternateViews.Add(htmlView);

            using (var smtp = new SmtpClient("smtp.mail.ru", 587))
            {
                smtp.Credentials = new NetworkCredential("profunion.kst@mail.ru", "QQ2ds83aRy3iMNvnSdqY");
               /* smtp.EnableSsl = true;*/
                smtp.Timeout = 10000; // 10 секунд


                await smtp.SendMailAsync(message);

            }
        }
    }
}
