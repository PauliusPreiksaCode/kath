using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace organization_back_end.Entities;

public class LicenceLedgerEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public DateTime PurchaseDate { get; set; }
    public bool IsActive { get; set; }
    
    [Required]
    public required Guid LicenceId { get; set; }
    [ForeignKey("LicenceId")]
    public Licence Licence { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    [ForeignKey("UserId")]
    public LicencedUser LicencedUser { get; set; }
    
    public Guid? PaymentId { get; set; }
    [ForeignKey("PaymentId")]
    public Payment? Payment { get; set; }
}