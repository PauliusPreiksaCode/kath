namespace organization_back_end.Auth.Model;

public class Roles
{
    public const string User = nameof(User);
    public const string LicencedUser = nameof(LicencedUser);
    public const string OrganizationOwner = nameof(OrganizationOwner);

    public static readonly IReadOnlyCollection<string> All = new[] { User, LicencedUser, OrganizationOwner };
}