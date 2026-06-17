using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for retrieving the next episode to watch for a given show, based on the user's watch progress. It uses an <see cref="IWatchTrackingProvider"/> to access the watch tracking data and determine the next episode in the series.
/// </summary>
/// <remarks>
/// Constructor for the GetNextEpisodeToWatchService class, which initializes the service with a specified watch tracking provider. The provider is used to retrieve watch progress data for shows and determine the next episode to watch.
/// </remarks>
/// <param name="watchTrackingProvider">Tracking provider</param>
/// <exception cref="ArgumentNullException"><see cref="IWatchTrackingProvider"/> is required</exception>
public sealed class GetNextEpisodeToWatchService(IWatchTrackingProvider watchTrackingProvider)
{
    /// <summary>
    /// Tracking provider used to retrieve watch progress data for shows. This provider is responsible for accessing the underlying data source and providing the necessary information to determine the next episode to watch.
    /// </summary>
    private readonly IWatchTrackingProvider _watchTrackingProvider = watchTrackingProvider
            ?? throw new ArgumentNullException(nameof(watchTrackingProvider));

    /// <summary>
    /// Gets the next episode to watch for a given show title. It retrieves the user's watch progress for the specified show and determines the next episode in the series. If there is no next episode available, it returns null.
    /// </summary>
    /// <param name="showTitle">Show title</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Show title is required</exception>
    public async Task<NextEpisodeToWatch?> GetNextEpisodeToWatchAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        var progress = await _watchTrackingProvider.GetShowProgressAsync(
            showTitle.Trim(),
            cancellationToken);

        if (progress?.NextSeason is null || progress.NextEpisode is null)
            return null;

        return new NextEpisodeToWatch(
            ProviderId: progress.ProviderId,
            ShowTitle: progress.ShowTitle,
            SeasonNumber: progress.NextSeason.Value,
            EpisodeNumber: progress.NextEpisode.Value,
            EpisodeTitle: progress.NextEpisodeTitle);
    }
}