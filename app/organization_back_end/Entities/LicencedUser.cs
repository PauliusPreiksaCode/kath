using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using organization_back_end.Auth.Model;

namespace organization_back_end.Entities;

public class LicencedUser : User
{
    public List<OrganizationUser>? OrganizationUsers { get; set; }
    public List<Entry>? Entries { get; set; }
    public List<LicenceLedgerEntry> LicenceLedgerEntries { get; set; }
}