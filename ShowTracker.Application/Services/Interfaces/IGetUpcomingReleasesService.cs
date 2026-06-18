using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Services.Interfaces
{
    public interface IGetUpcomingReleasesService
    {
        Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(CancellationToken cancellationToken = default);
    }
}