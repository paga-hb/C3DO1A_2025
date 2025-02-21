using System.ComponentModel.DataAnnotations;

namespace Flixtube.Web.Models;

public class VideoUploaded
{
    public string Message { get; set; } =  null!;
    public string FileName { get; set; } =  null!;
    public long FileSize { get; set; }
    public string VideoId { get; set; } =  null!;
}