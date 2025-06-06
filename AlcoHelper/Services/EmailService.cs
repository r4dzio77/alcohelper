using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace AlcoHelper.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void Send(string to, string subject, string body)
        {
            var smtp = _config.GetSection("SmtpSettings");
            var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
            {
                Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                EnableSsl = bool.Parse(smtp["EnableSsl"])
            };

            var message = new MailMessage(smtp["Username"], to, subject, body)
            {
                IsBodyHtml = true
            };

            client.Send(message);
        }
    }
}
