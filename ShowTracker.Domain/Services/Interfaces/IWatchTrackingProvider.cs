using ShowTracker.Domain.Models;

namespace ShowTracker.Domain.Services.Interfaces;

/// <summary>
/// Interface for a service that manages tracking of watched progress for TV shows and movies, allowing users to mark titles as watched and retrieve their watch progress. This abstraction allows for different implementations, such as in-memory tracking, database-backed tracking, or integration with external APIs.
/// </summary>
public interface IWatchTrackingProvider
{
    /// <summary>
    /// Marks a movie as watched by the user, allowing them to keep track of which movies they have seen. The method takes the movie's title and a cancellation token for asynchronous operation. It returns a Task representing the asynchronous operation, which completes when the movie is marked as watched.
    /// </summary>
    /// <param name="movieTitle">Movie title</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    Task MarkMovieWatchedAsync(
        string movieTitle,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a shows episode as watched by the user, allowing them to keep track of which episodes they have seen. The method takes the show's title, season number, episode number, and a cancellation token for asynchronous operation. It returns a Task representing the asynchronous operation, which completes when the episode is marked as watched.
    /// </summary>
    /// <param name="showTitle">Show title</param>
    /// <param name="seasonNumber">Season</param>
    /// <param name="episodeNumber">Episode</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    Task MarkEpisodeWatchedAsync(
        string showTitle,
        int seasonNumber,
        int episodeNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the watch progress for a specific show, allowing users to see which season and episode they last watched, as well as what the next episode is. The method takes the show's title and a cancellation token for asynchronous operation. It returns a Task that completes with a WatchProgress object containing the user's watch progress for the specified show, or null if no progress is found.
    /// </summary>
    /// <param name="showTitle">Show title</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    Task<WatchProgress?> GetShowProgressAsync(
        string showTitle,
        CancellationToken cancellationToken = default);
}
