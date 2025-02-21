using System.ComponentModel.DataAnnotations;

namespace Flixtube.Web.Models;

public class ViewHistory
{
    public int Id { get; set; }
    public string VideoId { get; set; } = null!;
    public DateTime ViewedAt { get; set; }
}