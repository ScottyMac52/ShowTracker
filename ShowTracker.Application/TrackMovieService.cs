using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for tracking a movie title. It uses an <see cref="ITitleTrackingProvider"/> to access the title tracking data and add a new movie title to the list of tracked titles for the user. The service validates the input movie title and platform, checks if the movie is already being tracked, and if not, it adds the movie to the tracking list and returns the tracked title information.
/// </summary>
/// <remarks>
/// Constructor for the TrackMovieService class, which initializes the service with a specified title tracking provider. The provider is used to access the title tracking data and add a new movie title to the list of tracked titles for the user.
/// </remarks>
/// <param name="titleTrackingProvider">Title Tracking provider</param>
/// <exception cref="ArgumentNullException"><see cref="ITitleTrackingProvider"/> is required</exception>
public sealed class TrackMovieService(ITitleTrackingProvider titleTrackingProvider)
{
    /// <summary>
    /// Title Tracking Provider used to access the title tracking data and add a new movie title to the list of tracked titles for the user. This provider is responsible for accessing the underlying data source and providing the necessary functionality to track a movie title.
    /// </summary>
    private readonly ITitleTrackingProvider _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));

    /// <summary>
    /// Tracks a movie title for the user. This method validates the input movie title and platform, checks if the movie is already being tracked, and if not, it adds the movie to the tracking list and returns the tracked title information. The method throws an exception if the input movie title is invalid or if the movie is already being tracked.
    /// </summary>
    /// <param name="movieTitle"></param>
    /// <param name="platform"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Movie title is required</exception>
    /// <exception cref="InvalidOperationException">Movie title is already being tracked!</exception>
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

        var existing = await _titleTrackingProvider.FindTrackedTitleAsync(
            normalizedTitle,
            cancellationToken);

        if (existing is not null)
            throw new InvalidOperationException($"'{normalizedTitle}' is already being tracked.");

        return await _titleTrackingProvider.TrackMovieAsync(
            normalizedTitle,
            normalizedPlatform,
            cancellationToken);
    }
}