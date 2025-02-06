using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace organization_back_end.Entities;

public class Organization
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    
    public List<Group>? Groups { get; set; }
    public List<OrganizationUser> Users { get; set; }
    
    [Required]
    public string OwnerId { get; set; }
    [ForeignKey("OwnerId")]
    public OrganizationOwner Owner { get; set; }
}