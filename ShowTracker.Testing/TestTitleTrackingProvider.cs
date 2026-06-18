using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ShowTracker.Testing;

/// <summary>
/// Implements a test double for the <see cref="ITitleTrackingProvider"/> interface, allowing tests to specify custom behavior for each method via delegate properties.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class TestTitleTrackingProvider : ITitleTrackingProvider
{
    /// <summary>
    /// Handler for the SearchTitlesAsync method. Tests can set this property to specify custom behavior for searching titles. If this handler is not set when the SearchTitlesAsync method is called, a NotImplementedException will be thrown.
    /// </summary>
    public Func<string, CancellationToken, Task<IReadOnlyList<TitleSearchResult>>>? SearchTitlesAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the TrackShowAsync method. Tests can set this property to specify custom behavior for tracking a show title. If this handler is not set when the TrackShowAsync method is called, a NotImplementedException will be thrown.
    /// </summary>
    public Func<string, string?, CancellationToken, Task<TrackedTitle>>? TrackShowAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the TrackMovieAsync method. Tests can set this property to specify custom behavior for tracking a movie title. If this handler is not set when the TrackMovieAsync method is called, a NotImplementedException will be thrown.
    /// </summary>
    public Func<string, string?, CancellationToken, Task<TrackedTitle>>? TrackMovieAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the UntrackAsync method. Tests can set this property to specify custom behavior for untracking a title. If this handler is not set when the UntrackAsync method is called, a NotImplementedException will be thrown.
    /// </summary>
    public Func<string, CancellationToken, Task>? UntrackAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the FindTrackedTitleAsync method. Tests can set this property to specify custom behavior for finding a tracked title by its title. If this handler is not set when the FindTrackedTitleAsync method is called, a NotImplementedException will be thrown.
    /// </summary>
    public Func<string, CancellationToken, Task<TrackedTitle?>>? FindTrackedTitleAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the GetTrackedTitlesAsync method. Tests can set this property to specify custom behavior for retrieving the list of tracked titles. If this handler is not set when the GetTrackedTitlesAsync method is called, a NotImplementedException will be thrown.
    /// </summary>
    public Func<CancellationToken, Task<IReadOnlyList<TrackedTitle>>>? GetTrackedTitlesAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the GetUpcomingReleasesAsync method. Tests can set this property to specify custom behavior for retrieving the list of upcoming releases. If this handler is not set when the GetUpcomingReleasesAsync method is called, a NotImplementedException will be thrown.
    /// </summary>
    public Func<CancellationToken, Task<IReadOnlyList<UpcomingRelease>>>? GetUpcomingReleasesAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the GetNextReleaseAsync method. Tests can set this property to specify custom behavior for retrieving the next release for a given show title. If this handler is not set when the GetNextReleaseAsync method is called, a NotImplementedException will be thrown.
    /// </summary>
    public Func<string, CancellationToken, Task<UpcomingRelease?>>? GetNextReleaseAsyncHandler { get; set; }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (SearchTitlesAsyncHandler is null)
            throw new NotImplementedException();

        return SearchTitlesAsyncHandler(query, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task<TrackedTitle> TrackShowAsync(
        string showTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (TrackShowAsyncHandler is null)
            throw new NotImplementedException();

        return TrackShowAsyncHandler(showTitle, platform, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task<TrackedTitle> TrackMovieAsync(
        string movieTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (TrackMovieAsyncHandler is null)
            throw new NotImplementedException();

        return TrackMovieAsyncHandler(movieTitle, platform, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task UntrackAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        if (UntrackAsyncHandler is null)
            throw new NotImplementedException();

        return UntrackAsyncHandler(title, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task<TrackedTitle?> FindTrackedTitleAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        if (FindTrackedTitleAsyncHandler is null)
            throw new NotImplementedException();

        return FindTrackedTitleAsyncHandler(title, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task<IReadOnlyList<TrackedTitle>> GetTrackedTitlesAsync(
        CancellationToken cancellationToken = default)
    {
        if (GetTrackedTitlesAsyncHandler is null)
            throw new NotImplementedException();

        return GetTrackedTitlesAsyncHandler(cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
        CancellationToken cancellationToken = default)
    {
        if (GetUpcomingReleasesAsyncHandler is null)
            throw new NotImplementedException();

        return GetUpcomingReleasesAsyncHandler(cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown if the corresponding handler is not set</exception>"
    public Task<UpcomingRelease?> GetNextReleaseAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (GetNextReleaseAsyncHandler is null)
            throw new NotImplementedException();

        return GetNextReleaseAsyncHandler(showTitle, cancellationToken);
    }
}
