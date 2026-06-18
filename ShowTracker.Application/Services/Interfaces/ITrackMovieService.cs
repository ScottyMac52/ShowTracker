using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Services.Interfaces
{
    public interface ITrackMovieService
    {
        Task<TrackedTitle> TrackMovieAsync(string movieTitle, string? platform = null, CancellationToken cancellationToken = default);
    }
}