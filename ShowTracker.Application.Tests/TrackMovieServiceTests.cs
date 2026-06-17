using ShowTracker.Domain.Models;

namespace ShowTracker.Application.Tests;

public sealed class TrackMovieServiceTests
{
    [Fact]
    public async Task TrackMovieAsync_Tracks_Movie_Through_Provider()
    {
        var trackedTitles = new List<TrackedTitle>();

        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackMovieAsyncHandler = (movieTitle, platform, _) =>
            {
                var movie = new TrackedTitle(
                    ProviderId: $"fake:movie:{movieTitle}",
                    Title: movieTitle,
                    Type: TrackedTitleType.Movie,
                    Platform: platform);

                trackedTitles.Add(movie);

                return Task.FromResult(movie);
            }
        };

        var service = new TrackMovieService(provider);

        var result = await service.TrackMovieAsync("Dune Part Two");

        Assert.Equal("Dune Part Two", result.Title);
        Assert.Equal(TrackedTitleType.Movie, result.Type);
        Assert.Single(trackedTitles);
    }

    [Fact]
    public async Task TrackMovieAsync_Rejects_Blank_Title()
    {
        var provider = new TestTitleTrackingProvider();
        var service = new TrackMovieService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.TrackMovieAsync(" "));
    }

    [Fact]
    public async Task TrackMovieAsync_Does_Not_Allow_Duplicate_Movies()
    {
        var trackedTitles = new List<TrackedTitle>();

        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (title, _) =>
            {
                var trackedTitle = trackedTitles.SingleOrDefault(t =>
                    string.Equals(t.Title, title, StringComparison.OrdinalIgnoreCase));

                return Task.FromResult(trackedTitle);
            },

            TrackMovieAsyncHandler = (movieTitle, platform, _) =>
            {
                var movie = new TrackedTitle(
                    ProviderId: $"fake:movie:{movieTitle}",
                    Title: movieTitle,
                    Type: TrackedTitleType.Movie,
                    Platform: platform);

                trackedTitles.Add(movie);

                return Task.FromResult(movie);
            }
        };

        var service = new TrackMovieService(provider);

        await service.TrackMovieAsync("Dune Part Two");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackMovieAsync("Dune Part Two"));
    }
}
