using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Gets the watch progress for a specific show, allowing users to see which season and episode they last watched, as well as what the next episode is. The service uses an <see cref="IWatchTrackingProvider"/> to access the watch tracking data and retrieve the user's watch progress for the specified show.
/// </summary>
/// <remarks>
/// Constructor for the GetShowProgressService class, which initializes the service with a specified watch tracking provider. The provider is used to retrieve watch progress data for shows and determine the user's watch progress for a specific show.
/// </remarks>
/// <param name="watchTrackingProvider">Tracking provider</param>
/// <exception cref="ArgumentNullException"><see cref="IWatchTrackingProvider"/> is required</exception>
public sealed class GetShowProgressService(IWatchTrackingProvider watchTrackingProvider)
{
    /// <summary>
    /// Tracking Provider used to retrieve watch progress data for shows. This provider is responsible for accessing the underlying data source and providing the necessary information to determine the user's watch progress for a specific show.
    /// </summary>
    private readonly IWatchTrackingProvider _watchTrackingProvider = watchTrackingProvider
            ?? throw new ArgumentNullException(nameof(watchTrackingProvider));

    /// <summary>
    /// Gets the watch progress for a specific show, allowing users to see which season and episode they last watched, as well as what the next episode is. The method uses the <see cref="IWatchTrackingProvider"/> to access the watch tracking data and retrieve the user's watch progress for the specified show.
    /// </summary>
    /// <param name="showTitle">Show title</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Show title is required</exception>
    public Task<WatchProgress?> GetShowProgressAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        return _watchTrackingProvider.GetShowProgressAsync(
            showTitle.Trim(),
            cancellationToken);
    }
}