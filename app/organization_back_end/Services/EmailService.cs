using MailKit.Net.Smtp;
using MimeKit;
using organization_back_end.Helpers;
using organization_back_end.Interfaces;
using organization_back_end.RequestDtos.Email;

namespace organization_back_end.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    private void SendEmail(SendEmailRequest request)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config["Smtp:Name"], _config["Smtp:SmtpServerUserName"]));
            message.To.Add(new MailboxAddress("", request.Recipient));
            message.Subject = request.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = request.Body
            };

            message.Body = builder.ToMessageBody();
            using var client = new SmtpClient();
            client.Connect(_config["Smtp:SmtpServer"], int.Parse(_config["Smtp:SmtpServerPort"] ?? "0"), true);
            client.Authenticate(_config["Smtp:SmtpServerUserName"], _config["Smtp:SmtpServerUserPassword"]);

            client.Send(message);
            client.Disconnect(true);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public void SendRegisterEmail(string email, string name)
    {
        var emailBody = EmailTemplates.GetRegisterEmailTemplate(name);
        
        var request = new SendEmailRequest
        {
            Recipient = email,
            Subject = "Welcome to KATH",
            Body = emailBody
        };
        
        SendEmail(request);
    }

    public void SendInvitationEmail(string organizationName, string inviterName, string inviteeName, string email)
    {
        var emailBody = EmailTemplates.GetOrganizationTeamInvitationTemplate(organizationName, inviterName, inviteeName);
        
        var request = new SendEmailRequest
        {
            Recipient = email,
            Subject = $"You have been invited to the organization",
            Body = emailBody
        };
        
        SendEmail(request);
    }
}