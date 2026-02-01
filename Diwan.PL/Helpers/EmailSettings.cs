using Diwan.DAL.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace Diwan.PL.Helpers
{
    public static class EmailSettings
    {
        private static readonly IConfigurationRoot _config;

        static EmailSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _config = builder.Build();
        }

        public static async Task SendEmailAsync(Email email)
        {
            var fromName = _config["EmailSettings:FromName"];
            var fromAddress = _config["EmailSettings:FromAddress"];
            var appPassword = _config["EmailSettings:AppPassword"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.Add(MailboxAddress.Parse(email.To));
            message.Subject = email.Subject;
            message.Body = new TextPart("html") { Text = email.Body };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromAddress, appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
