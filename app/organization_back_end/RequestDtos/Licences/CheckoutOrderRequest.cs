namespace organization_back_end.RequestDtos.Licences;

public class CheckoutOrderRequest
{
    public Guid licenceId { get; set; }
    public string UserId { get; set; }
}