using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Services.Interfaces;

public interface ISearchTitlesService
{
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