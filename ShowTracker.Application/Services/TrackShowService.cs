using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Services;

public sealed class TrackShowService : ITrackShowService
{
    private readonly ITitleTrackingProvider _titleTrackingProvider;
    private readonly ITrackedTitleRepository _trackedTitleRepository;

    public TrackShowService(
        ITitleTrackingProvider titleTrackingProvider,
        ITrackedTitleRepository trackedTitleRepository)
    {
        _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));

        _trackedTitleRepository = trackedTitleRepository
            ?? throw new ArgumentNullException(nameof(trackedTitleRepository));
    }

    public async Task<TrackedTitle> TrackShowAsync(
        string showTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        var normalizedTitle = showTitle.Trim();

        var normalizedPlatform = string.IsNullOrWhiteSpace(platform)
            ? null
            : platform.Trim();

        var trackedTitles = await _trackedTitleRepository.GetAllAsync(
            cancellationToken);

        if (trackedTitles.Any(title =>
            title.Type == TrackedTitleType.Show &&
            string.Equals(
                title.Title,
                normalizedTitle,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Show '{normalizedTitle}' is already tracked.");
        }

        var trackedTitle = await _titleTrackingProvider.TrackShowAsync(
            normalizedTitle,
            normalizedPlatform,
            cancellationToken);

        await _trackedTitleRepository.AddAsync(
            trackedTitle,
            cancellationToken);

        return trackedTitle;
    }
}