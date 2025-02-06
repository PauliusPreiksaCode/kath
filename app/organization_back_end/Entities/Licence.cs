using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using organization_back_end.Enums;

namespace organization_back_end.Entities;

public class Licence
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public LicenceType Type { get; set; }
    public int Duration { get; set; }
    public List<LicenceLedgerEntry>? LicenceLedgerEntries { get; set; }
}