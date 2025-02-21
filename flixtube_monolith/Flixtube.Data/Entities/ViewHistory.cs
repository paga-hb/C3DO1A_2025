using System.ComponentModel.DataAnnotations;

namespace Flixtube.Data.Entities;

public class ViewHistory
{
    [Key]
    public int Id { get; set; }

    [StringLength(125)]
    public string VideoId { get; set; } = null!;

    public DateTime ViewedAt { get; set; }
}