using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Services.Interfaces
{
    public interface IGetShowProgressService
    {
        Task<WatchProgress?> GetShowProgressAsync(string showTitle, CancellationToken cancellationToken = default);
    }
}