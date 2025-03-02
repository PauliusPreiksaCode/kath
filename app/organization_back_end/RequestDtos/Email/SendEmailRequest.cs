namespace organization_back_end.RequestDtos.Email;

public class SendEmailRequest
{
    public required string Recipient { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
}