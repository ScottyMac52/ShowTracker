using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Services;

public sealed class GetNextReleaseService : IGetNextReleaseService
{
    private readonly ITitleTrackingProvider _titleTrackingProvider;
    private readonly ITrackedTitleRepository _trackedTitleRepository;

    public GetNextReleaseService(
        ITitleTrackingProvider titleTrackingProvider,
        ITrackedTitleRepository trackedTitleRepository)
    {
        _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));

        _trackedTitleRepository = trackedTitleRepository
            ?? throw new ArgumentNullException(nameof(trackedTitleRepository));
    }

    public async Task<UpcomingRelease?> GetNextReleaseAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        var normalizedTitle = showTitle.Trim();

        var trackedTitles = await _trackedTitleRepository.GetAllAsync(cancellationToken);

        var trackedTitle = trackedTitles.FirstOrDefault(t =>
            string.Equals(t.Title, normalizedTitle, StringComparison.OrdinalIgnoreCase));

        if (trackedTitle is null)
            return null;

        return await _titleTrackingProvider.GetNextReleaseAsync(
            normalizedTitle,
            cancellationToken);
    }
}