using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace organization_back_end.Entities;

public class NoteBook
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public List<NotebookEntry>? Entries { get; set; }
    
    [Required]
    public Guid GroupId { get; set; }
    [ForeignKey("GroupId")]
    public Group Group { get; set; }
}