using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace organization_back_end.Entities;

public class File
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Extension { get; set; }
    public DateTime UploadDate { get; set; }
    
    [Required]
    public Guid EntryId { get; set; }
    [ForeignKey("EntryId")]
    public Entry Entry { get; set; }
}