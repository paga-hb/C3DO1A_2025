using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flixtube.Metadata.Entities;

public class Video
{
    [Key]
    [StringLength(125)]
    public string Id { get; set; } = null!;

    [Required]
    [StringLength(125)]
    public string Name { get; set; } = null!;
}