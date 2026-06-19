using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Providers.Trakt;

/// <summary>
/// Implements <see cref="ITitleTrackingProvider"/> to track titles using the Trakt API.
/// </summary>
public sealed class TraktTitleTrackingProvider : ITitleTrackingProvider
{
    /// <summary>
    /// Client for searching titles using the Trakt API.
    /// </summary>
    private readonly ITraktTitleSearchClient _titleSearchClient;

    private readonly ITraktReleaseClient _releaseClient;

    /// <summary>
    /// Constructor retained for existing tests and callers that only exercise search/track behavior.
    /// </summary>
    /// <param name="titleSearchClient"><see cref="ITraktTitleSearchClient"/></param>
    public TraktTitleTrackingProvider(
        ITraktTitleSearchClient titleSearchClient)
        : this(
              titleSearchClient,
              new MissingTraktReleaseClient())
    {
    }

    /// <summary>
    /// Constructor for the TraktTitleTrackingProvider.
    /// </summary>
    /// <param name="titleSearchClient"><see cref="ITraktTitleSearchClient"/></param>
    /// <param name="releaseClient"><see cref="ITraktReleaseClient"/></param>
    /// <exception cref="ArgumentNullException"><see cref="ITraktTitleSearchClient"/> is required</exception>
    public TraktTitleTrackingProvider(
        ITraktTitleSearchClient titleSearchClient,
        ITraktReleaseClient releaseClient)
    {
        _titleSearchClient = titleSearchClient
            ?? throw new ArgumentNullException(nameof(titleSearchClient));

        _releaseClient = releaseClient
            ?? throw new ArgumentNullException(nameof(releaseClient));
    }

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
        return _releaseClient.GetUpcomingReleasesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<UpcomingRelease?> GetNextReleaseAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        return _releaseClient.GetNextReleaseAsync(
            showTitle.Trim(),
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TrackedTitle?> FindTrackedTitleAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<TitleSearchResult>> SearchShowsAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        return _titleSearchClient.SearchShowsAsync(query, cancellationToken);
    }

    public Task<IReadOnlyList<TitleSearchResult>> SearchMoviesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        return _titleSearchClient.SearchMoviesAsync(query, cancellationToken);
    }

    private sealed class MissingTraktReleaseClient : ITraktReleaseClient
    {
        public Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException(
                $"{nameof(ITraktReleaseClient)} is required for release operations.");
        }

        public Task<UpcomingRelease?> GetNextReleaseAsync(
            string title,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException(
                $"{nameof(ITraktReleaseClient)} is required for release operations.");
        }
    }
}