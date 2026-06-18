using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Services;

public sealed class GetUpcomingReleasesService : IGetUpcomingReleasesService
{
    private readonly ITitleTrackingProvider _titleTrackingProvider;
    private readonly ITrackedTitleRepository _trackedTitleRepository;

    public GetUpcomingReleasesService(
        ITitleTrackingProvider titleTrackingProvider,
        ITrackedTitleRepository trackedTitleRepository)
    {
        _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));

        _trackedTitleRepository = trackedTitleRepository
            ?? throw new ArgumentNullException(nameof(trackedTitleRepository));
    }

    public async Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
        CancellationToken cancellationToken = default)
    {
        var trackedTitles = await _trackedTitleRepository.GetAllAsync(cancellationToken);

        if (trackedTitles.Count == 0)
            return [];

        var trackedProviderIds = trackedTitles
            .Select(t => t.ProviderId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var releases = await _titleTrackingProvider.GetUpcomingReleasesAsync(
            cancellationToken);

        return releases
            .Where(r => trackedProviderIds.Contains(r.ProviderId))
            .ToArray();
    }
}