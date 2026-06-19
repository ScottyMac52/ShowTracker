using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Services;

public sealed class TrackMovieService : ITrackMovieService
{
    private readonly ITitleTrackingProvider _titleTrackingProvider;
    private readonly ITrackedTitleRepository _trackedTitleRepository;

    public TrackMovieService(
        ITitleTrackingProvider titleTrackingProvider,
        ITrackedTitleRepository trackedTitleRepository)
    {
        _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));

        _trackedTitleRepository = trackedTitleRepository
            ?? throw new ArgumentNullException(nameof(trackedTitleRepository));
    }

    public async Task<TrackedTitle> TrackMovieAsync(
        string movieTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(movieTitle))
            throw new ArgumentException("Movie title is required.", nameof(movieTitle));

        var normalizedTitle = movieTitle.Trim();

        var normalizedPlatform = string.IsNullOrWhiteSpace(platform)
            ? null
            : platform.Trim();

        var trackedTitles = await _trackedTitleRepository.GetAllAsync(
            cancellationToken);

        if (trackedTitles.Any(title =>
            title.Type == TrackedTitleType.Movie &&
            string.Equals(
                title.Title,
                normalizedTitle,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Movie '{normalizedTitle}' is already tracked.");
        }

        var trackedTitle = await _titleTrackingProvider.TrackMovieAsync(
            normalizedTitle,
            normalizedPlatform,
            cancellationToken);

        await _trackedTitleRepository.AddAsync(
            trackedTitle,
            cancellationToken);

        return trackedTitle;
    }
}