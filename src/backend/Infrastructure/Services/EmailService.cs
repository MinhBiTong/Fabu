using Application.Interfaces;
using Domain.Configurations;
using Hangfire;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailConfiguration _mail;

        public EmailService(IOptions<MailConfiguration> mailSettings)
        {
            _mail = mailSettings.Value;
        }

        [AutomaticRetry(Attempts = 3)] //gioi han so lan gui lai email neu that bai
        public async Task SendEmailAsync(string toEmail, string subject, string email_message)
        {
            try
            {
                var email = new MimeMessage();

                email.From.Add(new MailboxAddress(_mail.SenderName, _mail.From.Trim()));
                email.To.Add(MailboxAddress.Parse(toEmail.Trim()));
                email.Subject = subject;

                email.Body = new TextPart("html")
                {
                    Text = email_message
                };

                var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await smtp.ConnectAsync(_mail.Host, _mail.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mail.Username, _mail.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                Console.WriteLine($"FROM: {_mail.From}");
                Console.WriteLine($"TO: {toEmail}");
            }
            catch (SmtpCommandException ex)
            {
                throw;// lỗi server → retry
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message); // email sai → KHÔNG retry
            }
        }
    }
}
