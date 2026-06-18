using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Services.Interfaces
{
    public interface IGetNextEpisodeToWatchService
    {
        Task<NextEpisodeToWatch?> GetNextEpisodeToWatchAsync(string showTitle, CancellationToken cancellationToken = default);
    }
}