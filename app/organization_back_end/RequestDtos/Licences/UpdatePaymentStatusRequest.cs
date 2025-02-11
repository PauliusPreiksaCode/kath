namespace organization_back_end.RequestDtos.Licences;

public class UpdatePaymentStatusRequest
{
    public required string SessionId { get; set; }
    public Guid LedgerId { get; set; }
}