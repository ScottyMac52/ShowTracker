using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Services;

public sealed class SearchTitlesService : ISearchTitlesService
{
    private readonly ITitleTrackingProvider _titleTrackingProvider;

    public SearchTitlesService(ITitleTrackingProvider titleTrackingProvider)
    {
        _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));
    }

    public Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Search query is required.", nameof(query));

        return _titleTrackingProvider.SearchTitlesAsync(
            query.Trim(),
            cancellationToken);
    }
}