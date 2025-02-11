using organization_back_end.Enums;

namespace organization_back_end.RequestDtos.Licences;

public record CreateLicenceRequest
{
    public string Name { get; init; }
    public decimal Price { get; init; }
    public LicenceType Type { get; init; }
    public int Duration { get; init; }
}