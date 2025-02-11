using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace organization_back_end.Entities;

public class Payment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public required string PaymentNumberStripe { get; set; }
    
    [Required]
    public Guid LicenceLegerEntryId { get; set; }
    [ForeignKey("LicenceLegerEntryId")]
    public LicenceLedgerEntry LicenceLegerEntry { get; set; }
}