using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Service.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ILogger<EmailSenderService> _logger; 
        private readonly IConfiguration _configuration;
        public EmailSenderService(ILogger<EmailSenderService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmail(string toEmail, string defaultPassword)
        {
            string mailServer = _configuration["EmailSettings:MailServer"]!;
            string fromEmail = _configuration["EmailSettings:FromEmail"]!;
            string password = _configuration["EmailSettings:Password"]!;
            string senderName = _configuration["EmailSettings:SenderName"]!;
            int mailPort = Int32.Parse(_configuration["EmailSettings:MailPort"]!);

            try
            {
                var client = new SmtpClient(mailServer, mailPort)
                {
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true,
                };

                MailAddress fromAddress = new MailAddress(fromEmail, senderName);
                MailMessage mailMessage = new MailMessage
                {
                    From = fromAddress,
                    Subject = "Recovery Password",
                    Body = $"Your Temporary Password is {defaultPassword}", 
                };

                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

    }
}
