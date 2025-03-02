namespace organization_back_end.Helpers;

public static class EmailTemplates
{
    public static string GetRegisterEmailTemplate(string name)
    {
        var template = @"
<!DOCTYPE html>
<html lang='en'>

<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>

<table style='max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 8px;'>
    <tr>
        <td>
            <h1>Welcome to KATH!</h1>
            <p>{0},</p>
            <p>Thank you for registering with KATH! We're excited to have you on board. Get ready to explore all the great features we have to offer.</p>
            <p>If you have any questions or need assistance, feel free to reach out to our support team.</p>
            <br>
            <p>Best regards,</p>
            <p>KATH team</p>
        </td>
    </tr>
</table>

</body>

</html>
";
        var emailTemplate = string.Format(template, name);
        
        return emailTemplate;
    }

    public static string GetOrganizationTeamInvitationTemplate(string organizationName, string inviterName, string inviteeName)
    {
        var template = @"
<!DOCTYPE html>
<html lang='en'>

<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>

<table style='max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 8px;'>
    <tr>
        <td>
            <h1>You have been invited to the organization {0}!</h1>
            <p>Dear {1},</p>
            <p><strong>{2}</strong> has invited you to join the organization <strong>{0}</strong> on KATH.</p>
        </td>
    </tr>
</table>

</body>

</html>
";
        var emailTemplate = string.Format(template, organizationName, inviteeName, inviterName);

        return emailTemplate;
    }


}