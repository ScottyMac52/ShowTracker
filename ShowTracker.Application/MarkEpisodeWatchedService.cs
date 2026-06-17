using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for marking an episode as watched for a specific show. It uses an <see cref="IWatchTrackingProvider"/> to access the watch tracking data and update the user's watch progress for the specified show, season, and episode.
/// </summary>
/// <remarks>
/// Constructor for the MarkEpisodeWatchedService class, which initializes the service with a specified watch tracking provider. The provider is used to access the watch tracking data and update the user's watch progress for the specified show, season, and episode.
/// </remarks>
/// <param name="watchTrackingProvider">Watch Tracking provider</param>
/// <exception cref="ArgumentNullException"><see cref="IWatchTrackingProvider"/> is required</exception>
public sealed class MarkEpisodeWatchedService(IWatchTrackingProvider watchTrackingProvider)
{
    /// <summary>
    /// Tracking provider used to access the watch tracking data and update the user's watch progress for the specified show, season, and episode. This provider is responsible for accessing the underlying data source and providing the necessary functionality to mark an episode as watched.
    /// </summary>
    private readonly IWatchTrackingProvider _watchTrackingProvider = watchTrackingProvider
            ?? throw new ArgumentNullException(nameof(watchTrackingProvider));

    /// <summary>
    /// Gets the next episode to watch for a given show title. It retrieves the user's watch progress for the specified show and determines the next episode in the series. If there is no next episode available, it returns null.
    /// </summary>
    /// <param name="showTitle">Show title</param>
    /// <param name="seasonNumber">Season</param>
    /// <param name="episodeNumber">Episode</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Show title is required</exception>
    /// <exception cref="ArgumentOutOfRangeException">Season and Episode must be > 0</exception>
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

        return _watchTrackingProvider.MarkEpisodeWatchedAsync(
            showTitle.Trim(),
            seasonNumber,
            episodeNumber,
            cancellationToken);
    }
}