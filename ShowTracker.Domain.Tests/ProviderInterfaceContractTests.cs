using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Domain.Tests;

public sealed class ProviderInterfaceContractTests
{
    [Fact]
    public async Task TitleTrackingProvider_Can_Track_Show()
    {
        var trackedTitles = new List<TrackedTitle>();

        ITitleTrackingProvider provider = new TestTitleTrackingProvider
        {
            TrackShowAsyncHandler = (showTitle, platform, _) =>
            {
                var title = new TrackedTitle(
                    ProviderId: $"fake:show:{showTitle}",
                    Title: showTitle,
                    Type: TrackedTitleType.Show,
                    Platform: platform);

                trackedTitles.Add(title);

                return Task.FromResult(title);
            }
        };

        var result = await provider.TrackShowAsync("Your Friends & Neighbors", "Apple TV");

        Assert.Equal("Your Friends & Neighbors", result.Title);
        Assert.Equal(TrackedTitleType.Show, result.Type);
        Assert.Equal("Apple TV", result.Platform);
        Assert.Single(trackedTitles);
    }

    [Fact]
    public async Task WatchTrackingProvider_Can_Mark_Episode_Watched()
    {
        IWatchTrackingProvider provider = new FakeWatchTrackingProvider();

        await provider.MarkEpisodeWatchedAsync("Andor", 2, 5);

        var progress = await provider.GetShowProgressAsync("Andor");

        Assert.NotNull(progress);
        Assert.Equal(2, progress.LastWatchedSeason);
        Assert.Equal(5, progress.LastWatchedEpisode);
    }

    private sealed class TestTitleTrackingProvider : ITitleTrackingProvider
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

    private sealed class FakeWatchTrackingProvider : IWatchTrackingProvider
    {
        private WatchProgress? _progress;

        public Task MarkMovieWatchedAsync(
            string movieTitle,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task MarkEpisodeWatchedAsync(
            string showTitle,
            int seasonNumber,
            int episodeNumber,
            CancellationToken cancellationToken = default)
        {
            _progress = new WatchProgress(
                ProviderId: $"fake:show:{showTitle}",
                ShowTitle: showTitle,
                LastWatchedSeason: seasonNumber,
                LastWatchedEpisode: episodeNumber,
                LastWatchedEpisodeTitle: null,
                NextSeason: null,
                NextEpisode: null,
                NextEpisodeTitle: null);

            return Task.CompletedTask;
        }

        public Task<WatchProgress?> GetShowProgressAsync(
            string showTitle,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_progress);
        }
    }
}
