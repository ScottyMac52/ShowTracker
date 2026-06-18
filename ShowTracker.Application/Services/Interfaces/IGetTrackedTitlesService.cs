using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Services.Interfaces
{
    public interface IGetTrackedTitlesService
    {
        Task<IReadOnlyList<TrackedTitle>> GetTrackedTitlesAsync(CancellationToken cancellationToken = default);
    }
}