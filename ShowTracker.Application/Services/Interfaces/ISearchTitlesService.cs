using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Services.Interfaces
{
    public interface ISearchTitlesService
    {
        Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(string query, CancellationToken cancellationToken = default);
    }
}