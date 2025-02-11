namespace organization_back_end.RequestDtos.Licences;

public class TransferLicenceRequest
{
    public required string NewUserId { get; set; }
    public Guid LicenceId { get; set; }
}