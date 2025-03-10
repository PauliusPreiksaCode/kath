using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace organization_back_end.Entities;

public class Entry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Text { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModifyDate { get; set; }
    
    public Guid? FileId { get; set; }
    [ForeignKey("FileId")]
    public File? File { get; set; }
    
    [Required]
    public Guid GroupId { get; set; }
    [ForeignKey("GroupId")]
    [JsonIgnore]
    public Group Group { get; set; }
    
    [Required]
    public string LicencedUserId { get; set; }
    [ForeignKey("LicencedUserId")]
    [JsonIgnore]
    public LicencedUser LicencedUser { get; set; }
}