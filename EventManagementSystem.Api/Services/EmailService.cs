using System.Net.Mail;
using System.Net;
using System.Runtime;
using Microsoft.Extensions.Options;
using EventManagementSystem.Api.Common.AppSettings;
using EventManagementSystem.Api.Services.Interfaces;

namespace EventManagementSystem.Api.Services;

public class EmailService: IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _settings = emailSettings.Value;
        _logger = logger;
    }

    public void SendEventRegistrationEmail(string firstName, string email, string eventTitle)
    {
        try
        {
            MailAddress to = new MailAddress(email);
            MailAddress from = new MailAddress(_settings.Sender.ToString());
            var mail = new MailMessage(from, to);
            mail.Subject = "Registration Successful";

            mail.Body = EventRegistrationTemplate
                            .Replace("{firstName}", firstName)
                            .Replace("{event}", eventTitle);
            mail.IsBodyHtml = true;

            SendEmail(mail);
        }
        catch (Exception ex)
        {
            _logger.LogError("[SendNewMemberEmail] - Exception - {0}\n {1}", ex.Message, ex.StackTrace);
        }

    }

    public void SendEmail(MailMessage mail)
    {
        using (var smtpClient = new SmtpClient(_settings.Host, _settings.Port))
        {
            smtpClient.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            smtpClient.EnableSsl = true;
            smtpClient.Send(mail);
            smtpClient.Dispose();

        }
    }

    private static string EventRegistrationTemplate = @"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Event Registration</title>
  </head>

  <body>
    <div class=""main-container""><p>Hi {firstName}.</p>
             <p>Thank you for registering for event: {event}</p> 
            <p> See you there...</p>
         
    </div>
  </body>
</html>
";
}
