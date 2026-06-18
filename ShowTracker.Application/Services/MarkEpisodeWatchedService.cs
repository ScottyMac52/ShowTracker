using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Services;

public sealed class MarkEpisodeWatchedService : IMarkEpisodeWatchedService
{
    private readonly IWatchProgressRepository _watchProgressRepository;

    public MarkEpisodeWatchedService(IWatchProgressRepository watchProgressRepository)
    {
        _watchProgressRepository = watchProgressRepository
            ?? throw new ArgumentNullException(nameof(watchProgressRepository));
    }

    public Task MarkEpisodeWatchedAsync(
        string showTitle,
        int seasonNumber,
        int episodeNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        if (seasonNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(seasonNumber));

        if (episodeNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(episodeNumber));

        var progress = new WatchProgress(
            ProviderId: string.Empty,
            ShowTitle: showTitle.Trim(),
            LastWatchedSeason: seasonNumber,
            LastWatchedEpisode: episodeNumber,
            LastWatchedEpisodeTitle: null,
            NextSeason: null,
            NextEpisode: null,
            NextEpisodeTitle: null);

        return _watchProgressRepository.SaveAsync(
            progress,
            cancellationToken);
    }
}