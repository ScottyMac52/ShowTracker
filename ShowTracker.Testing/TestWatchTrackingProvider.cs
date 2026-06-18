using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ShowTracker.Testing;

/// <summary>
/// Test double for the <see cref="IWatchTrackingProvider"/> interface, allowing tests to specify custom behavior for each method via delegate properties. This class is used in unit tests to mock the behavior of a watch tracking provider and verify that the application logic interacts with it correctly.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class TestWatchTrackingProvider : IWatchTrackingProvider
{
    /// <summary>
    /// Handler for marking a movie as watched. This delegate property allows tests to specify custom behavior for the MarkMovieWatchedAsync method, enabling verification of interactions with the watch tracking provider when marking a movie as watched.
    /// </summary>
    public Func<string, CancellationToken, Task>? MarkMovieWatchedAsyncHandler { get; set; }

    /// <summary>
    /// Handler for marking an episode as watched. This delegate property allows tests to specify custom behavior for the MarkEpisodeWatchedAsync method, enabling verification of interactions with the watch tracking provider when marking an episode as watched for a specific show, season, and episode number.
    /// </summary>
    public Func<string, int, int, CancellationToken, Task>? MarkEpisodeWatchedAsyncHandler { get; set; }

    /// <summary>
    /// Handler for getting show progress. This delegate property allows tests to specify custom behavior for the GetShowProgressAsync method, enabling verification of interactions with the watch tracking provider when retrieving the watch progress for a specific show title.
    /// </summary>
    public Func<string, CancellationToken, Task<WatchProgress?>>? GetShowProgressAsyncHandler { get; set; }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task MarkMovieWatchedAsync(
        string movieTitle,
        CancellationToken cancellationToken = default)
    {
        if (MarkMovieWatchedAsyncHandler is null)
            throw new NotImplementedException();

        return MarkMovieWatchedAsyncHandler(movieTitle, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task MarkEpisodeWatchedAsync(
        string showTitle,
        int seasonNumber,
        int episodeNumber,
        CancellationToken cancellationToken = default)
    {
        if (MarkEpisodeWatchedAsyncHandler is null)
            throw new NotImplementedException();

        return MarkEpisodeWatchedAsyncHandler(
            showTitle,
            seasonNumber,
            episodeNumber,
            cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task<WatchProgress?> GetShowProgressAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (GetShowProgressAsyncHandler is null)
            throw new NotImplementedException();

        return GetShowProgressAsyncHandler(showTitle, cancellationToken);
    }
}