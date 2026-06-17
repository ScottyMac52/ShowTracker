using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Tests;

/// <summary>
/// Implements a test double for the <see cref="ITitleTrackingProvider"/> interface, allowing tests to specify custom behavior for each method via delegate properties.
/// </summary>
internal sealed class TestTitleTrackingProvider : ITitleTrackingProvider
{
    public Func<string, CancellationToken, Task<IReadOnlyList<TitleSearchResult>>>? SearchTitlesAsyncHandler { get; set; }

    public Func<string, string?, CancellationToken, Task<TrackedTitle>>? TrackShowAsyncHandler { get; set; }

    public Func<string, string?, CancellationToken, Task<TrackedTitle>>? TrackMovieAsyncHandler { get; set; }

    public Func<string, CancellationToken, Task>? UntrackAsyncHandler { get; set; }

    public Func<string, CancellationToken, Task<TrackedTitle?>>? FindTrackedTitleAsyncHandler { get; set; }

    public Func<CancellationToken, Task<IReadOnlyList<TrackedTitle>>>? GetTrackedTitlesAsyncHandler { get; set; }

    public Func<CancellationToken, Task<IReadOnlyList<UpcomingRelease>>>? GetUpcomingReleasesAsyncHandler { get; set; }

    public Func<string, CancellationToken, Task<UpcomingRelease?>>? GetNextReleaseAsyncHandler { get; set; }

    public Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (SearchTitlesAsyncHandler is null)
            throw new NotImplementedException();

        return SearchTitlesAsyncHandler(query, cancellationToken);
    }

    public Task<TrackedTitle> TrackShowAsync(
        string showTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (TrackShowAsyncHandler is null)
            throw new NotImplementedException();

        return TrackShowAsyncHandler(showTitle, platform, cancellationToken);
    }

    public Task<TrackedTitle> TrackMovieAsync(
        string movieTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (TrackMovieAsyncHandler is null)
            throw new NotImplementedException();

        return TrackMovieAsyncHandler(movieTitle, platform, cancellationToken);
    }

    public Task UntrackAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        if (UntrackAsyncHandler is null)
            throw new NotImplementedException();

        return UntrackAsyncHandler(title, cancellationToken);
    }

    public Task<TrackedTitle?> FindTrackedTitleAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        if (FindTrackedTitleAsyncHandler is null)
            throw new NotImplementedException();

        return FindTrackedTitleAsyncHandler(title, cancellationToken);
    }

    public Task<IReadOnlyList<TrackedTitle>> GetTrackedTitlesAsync(
        CancellationToken cancellationToken = default)
    {
        if (GetTrackedTitlesAsyncHandler is null)
            throw new NotImplementedException();

        return GetTrackedTitlesAsyncHandler(cancellationToken);
    }

    public Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
        CancellationToken cancellationToken = default)
    {
        if (GetUpcomingReleasesAsyncHandler is null)
            throw new NotImplementedException();

        return GetUpcomingReleasesAsyncHandler(cancellationToken);
    }

    public Task<UpcomingRelease?> GetNextReleaseAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (GetNextReleaseAsyncHandler is null)
            throw new NotImplementedException();

        return GetNextReleaseAsyncHandler(showTitle, cancellationToken);
    }
}
