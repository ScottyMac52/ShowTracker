using ShowTracker.Domain.Models;

namespace ShowTracker.Domain.Services.Interfaces;

/// <summary>
/// Interface for a service that manages tracking of TV shows and movies, allowing users to track titles, untrack them, retrieve tracked titles, and get information about upcoming releases. This abstraction allows for different implementations, such as in-memory tracking, database-backed tracking, or integration with external APIs.
/// </summary>
public interface ITitleTrackingProvider
{
    /// <summary>
    /// Sets a show as tracked by the user, allowing them to receive updates about its release schedule and other relevant information. The method takes the show's title, an optional platform (e.g., Netflix, Hulu), and a cancellation token for asynchronous operation. It returns a TrackedTitle object representing the tracked show.
    /// </summary>
    /// <param name="showTitle"></param>
    /// <param name="platform"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TrackedTitle> TrackShowAsync(
        string showTitle,
        string? platform = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a movie as tracked by the user, allowing them to receive updates about its release schedule and other relevant information. The method takes the movie's title, an optional platform (e.g., Netflix, Hulu), and a cancellation token for asynchronous operation. It returns a TrackedTitle object representing the tracked movie.
    /// </summary>
    /// <param name="movieTitle"></param>
    /// <param name="platform"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TrackedTitle> TrackMovieAsync(
        string movieTitle,
        string? platform = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Untracks a title (either a show or a movie) by its title, removing it from the user's tracked list. The method takes the title to untrack and a cancellation token for asynchronous operation. It returns a Task representing the asynchronous operation, which completes when the untracking is done.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UntrackAsync(
        string title,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all titles currently tracked by the user, including both shows and movies. The method takes a cancellation token for asynchronous operation and returns a read-only list of TrackedTitle objects representing the user's tracked titles.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TrackedTitle>> GetTrackedTitlesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of upcoming releases for the tracked titles, providing information about the release date, title, type (show or movie), and other relevant details. The method takes a cancellation token for asynchronous operation and returns a read-only list of UpcomingRelease objects representing the upcoming releases for the user's tracked titles.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next upcoming release for a specific tracked title, providing information about the release date, title, type (show or movie), and other relevant details. The method takes the show's title and a cancellation token for asynchronous operation, and returns an UpcomingRelease object representing the next upcoming release for the specified tracked title, or null if there are no upcoming releases.
    /// </summary>
    /// <param name="showTitle"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UpcomingRelease?> GetNextReleaseAsync(
        string showTitle,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a tracked title by its title, allowing the user to check if a specific show or movie is currently being tracked. The method takes the title to search for and a cancellation token for asynchronous operation, and returns a TrackedTitle object representing the found tracked title, or null if the title is not currently being tracked.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TrackedTitle?> FindTrackedTitleAsync(
        string title,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for titles (both shows and movies) based on a query string, allowing the user to find potential titles to track. The method takes a search query and a cancellation token for asynchronous operation, and returns a read-only list of TitleSearchResult objects representing the search results that match the query.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
        string query,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TitleSearchResult>> SearchShowsAsync(
    string query,
    CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TitleSearchResult>> SearchMoviesAsync(
        string query,
        CancellationToken cancellationToken = default);
}
