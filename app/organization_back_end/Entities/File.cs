using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace organization_back_end.Entities;

public class File
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Extension { get; set; }
    public DateTime UploadDate { get; set; }
    public required string Path { get; set; }
    public required string Url { get; set; }
    
    [Required]
    public Guid EntryId { get; set; }
    [ForeignKey("EntryId")]
    [JsonIgnore]
    public Entry Entry { get; set; }
}