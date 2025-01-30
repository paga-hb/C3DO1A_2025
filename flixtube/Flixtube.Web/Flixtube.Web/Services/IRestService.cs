using Flixtube.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Flixtube.Web.Services;

public interface IRestService
{
    Task<List<Video>> GetMetadataAsync();
    Task<Video> GetMetadataAsync(string id);
    Task<List<ViewHistory>> GetViewingHistoryAsync();
    Task<bool> UploadVideoAsync(IBrowserFile file);
    Task DeleteVideoAsync(string id);
}