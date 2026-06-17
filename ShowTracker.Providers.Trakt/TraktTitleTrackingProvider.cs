using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Providers.Trakt;

/// <summary>
/// Implements <see cref="ITitleTrackingProvider"/> to track titles using the Trakt API.
/// </summary>
/// <remarks>
/// Constructor for the TraktTitleTrackingProvider.
/// </remarks>
/// <param name="titleSearchClient"><see cref="ITraktTitleSearchClient"/></param>
/// <exception cref="ArgumentNullException"><see cref="ITraktTitleSearchClient"/> is required</exception>
public sealed class TraktTitleTrackingProvider(ITraktTitleSearchClient titleSearchClient) : ITitleTrackingProvider
{
    /// <summary>
    /// Client for searching titles using the Trakt API.
    /// </summary>
    private readonly ITraktTitleSearchClient _titleSearchClient = titleSearchClient
            ?? throw new ArgumentNullException(nameof(titleSearchClient));

    /// <inheritdoc/>
    public Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        return _titleSearchClient.SearchTitlesAsync(query, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TrackedTitle> TrackShowAsync(
        string showTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        var normalizedTitle = showTitle.Trim();

        var results = await _titleSearchClient.SearchTitlesAsync(
            normalizedTitle,
            cancellationToken);

        var match = results.FirstOrDefault(r =>
            r.Type == TrackedTitleType.Show &&
            string.Equals(r.Title, normalizedTitle, StringComparison.OrdinalIgnoreCase));

        if (match is null)
            throw new InvalidOperationException($"Show '{normalizedTitle}' was not found.");

        return new TrackedTitle(
            ProviderId: match.ProviderId,
            Title: match.Title,
            Type: TrackedTitleType.Show,
            Platform: string.IsNullOrWhiteSpace(platform) ? null : platform.Trim());
    }

    /// <inheritdoc/>
    public async Task<TrackedTitle> TrackMovieAsync(
        string movieTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(movieTitle))
            throw new ArgumentException("Movie title is required.", nameof(movieTitle));

        var normalizedTitle = movieTitle.Trim();

        var results = await _titleSearchClient.SearchTitlesAsync(
            normalizedTitle,
            cancellationToken);

        var match = results.FirstOrDefault(r =>
            r.Type == TrackedTitleType.Movie &&
            string.Equals(r.Title, normalizedTitle, StringComparison.OrdinalIgnoreCase));

        if (match is null)
            throw new InvalidOperationException($"Movie '{normalizedTitle}' was not found.");

        return new TrackedTitle(
            ProviderId: match.ProviderId,
            Title: match.Title,
            Type: TrackedTitleType.Movie,
            Platform: string.IsNullOrWhiteSpace(platform) ? null : platform.Trim());
    }

    /// <inheritdoc/>
    public Task UntrackAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<TrackedTitle>> GetTrackedTitlesAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<UpcomingRelease?> GetNextReleaseAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<TrackedTitle?> FindTrackedTitleAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}