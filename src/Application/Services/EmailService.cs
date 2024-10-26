using System.Net.Mail;
using System.Net;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using static Application.Models.Email.ProjectEnum;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _senderEmail;
        private readonly string _senderName;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _host = _configuration["SmtpSettings:Host"];
            _port = int.Parse(_configuration["SmtpSettings:Port"]);
            _username = _configuration["SmtpSettings:Username"];
            _password = _configuration["SmtpSettings:Password"];
            _senderEmail = _configuration["SmtpSettings:SenderEmail"];
            _senderName = _configuration["SmtpSettings:SenderName"];
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_host)
                {
                    Port = _port,
                    Credentials = new NetworkCredential(_username, _password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail, _senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        
    }
}
