using ShowTracker.Domain.Models;

namespace ShowTracker.Providers.Trakt;

public interface ITraktTitleSearchClient
{
    /// <summary>
    /// Searches for titles using the Trakt API.
    /// </summary>
    /// <param name="query">Query to use for the search</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
        string query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for shows using the Trakt API.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TitleSearchResult>> SearchShowsAsync(
    string query,
    CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for movies using the Trakt API.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TitleSearchResult>> SearchMoviesAsync(
        string query,
        CancellationToken cancellationToken = default);
}