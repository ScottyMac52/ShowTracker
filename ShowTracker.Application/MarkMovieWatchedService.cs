using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for marking a movie as watched. It uses an <see cref="IWatchTrackingProvider"/> to access the watch tracking data and update the user's watch progress for the specified movie.
/// </summary>
/// <remarks>
/// Constructor for the MarkMovieWatchedService class, which initializes the service with a specified watch tracking provider. The provider is used to access the watch tracking data and update the user's watch progress for the specified movie.
/// </remarks>
/// <param name="watchTrackingProvider"></param>
/// <exception cref="ArgumentNullException"><see cref="IWatchTrackingProvider"/> is required</exception>
public sealed class MarkMovieWatchedService(IWatchTrackingProvider watchTrackingProvider)
{
    /// <summary>
    /// Watch Tracking Provider used to access the watch tracking data and update the user's watch progress for the specified movie. This provider is responsible for accessing the underlying data source and providing the necessary functionality to mark a movie as watched.
    /// </summary>
    private readonly IWatchTrackingProvider _watchTrackingProvider = watchTrackingProvider
            ?? throw new ArgumentNullException(nameof(watchTrackingProvider));

    /// <summary>
    /// Marks a movie as watched for the user. It updates the user's watch progress for the specified movie using the <see cref="IWatchTrackingProvider"/>. If the movie title is null or whitespace, an <see cref="ArgumentException"/> is thrown.
    /// </summary>
    /// <param name="movieTitle"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">MOvie title is required</exception>
    public Task MarkMovieWatchedAsync(
        string movieTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(movieTitle))
            throw new ArgumentException("Movie title is required.", nameof(movieTitle));

        return _watchTrackingProvider.MarkMovieWatchedAsync(
            movieTitle.Trim(),
            cancellationToken);
    }
}