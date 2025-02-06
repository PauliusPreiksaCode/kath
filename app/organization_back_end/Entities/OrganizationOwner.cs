namespace organization_back_end.Entities;

public class OrganizationOwner : LicencedUser
{
    public List<Organization>? Organizations { get; set; }
}