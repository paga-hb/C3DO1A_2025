using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flixtube.Data.Entities;

public class Video
{
    [Key]
    [StringLength(125)]
    public string Id { get; set; } = null!;

    [Required]
    [StringLength(125)]
    public string Name { get; set; } = null!;
}