using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Services.Interfaces
{
    public interface IGetNextReleaseService
    {
        Task<UpcomingRelease?> GetNextReleaseAsync(string showTitle, CancellationToken cancellationToken = default);
    }
}