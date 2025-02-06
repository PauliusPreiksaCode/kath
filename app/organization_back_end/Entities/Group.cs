using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace organization_back_end.Entities;

public class Group
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    
    [Required]
    public Guid OrganizationId { get; set; }
    [ForeignKey("OrganizationId")]
    public Organization Organization { get; set; }
    
    public Guid? NoteBookId { get; set; }
    [ForeignKey("NoteBookId")]
    public NoteBook? NoteBook { get; set; }
    
    public List<Entry>? Entries { get; set; }
}