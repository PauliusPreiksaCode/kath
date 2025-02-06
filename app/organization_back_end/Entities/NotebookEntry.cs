using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace organization_back_end.Entities;

public class NotebookEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int No { get; set; }
    public required string Text { get; set; }
    public DateTime CreationDate { get; set; }
    
    [Required]
    public Guid NoteBookId { get; set; }
    [ForeignKey("NoteBookId")]
    public NoteBook NoteBook { get; set; }
}