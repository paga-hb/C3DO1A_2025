using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flixtube.Gateway.Models;

public class ViewHistory
{
    public int Id { get; set; }
    public string VideoId { get; set; } = null!;
    public DateTime ViewedAt { get; set; }
}