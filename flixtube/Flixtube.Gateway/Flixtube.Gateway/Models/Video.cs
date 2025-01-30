using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flixtube.Gateway.Models;

public class Video
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}