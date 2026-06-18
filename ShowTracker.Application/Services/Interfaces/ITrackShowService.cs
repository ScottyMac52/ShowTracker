using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Services.Interfaces
{
    public interface ITrackShowService
    {
        Task<TrackedTitle> TrackShowAsync(string showTitle, string? platform = null, CancellationToken cancellationToken = default);
    }
}