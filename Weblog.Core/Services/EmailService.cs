using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Weblog.Core.ServiceContracts;

namespace Weblog.Core.Services
{
	public class EmailService : IEmailService
	{
		IConfiguration _config;

		public EmailService(IConfiguration config) { _config = config; }

		public async Task SendEmailAsync(string to, string subject, string body)
		{
			var config = _config.GetSection("EmailSettings");
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(config["SenderName"], config["SenderEmail"]));
			message.To.Add(new MailboxAddress("", to));
			message.Subject = subject;
			message.Body = new TextPart("html") { Text = body };

			using (var client = new SmtpClient())
			{
				await client.ConnectAsync(config["SmtpServer"], Convert.ToInt32(config["SmtpPort"]), true);
				await client.AuthenticateAsync(config["SmtpUser"], config["SmtpPass"]);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);
			}
		}
	}
}
