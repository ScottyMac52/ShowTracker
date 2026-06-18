using ShowTracker.Application.Services;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

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

        var service = new TrackMovieService(provider, CreateNoOpRepository());

        var result = await service.TrackMovieAsync("Dune Part Two");

        Assert.Equal("Dune Part Two", result.Title);
        Assert.Equal(TrackedTitleType.Movie, result.Type);
        Assert.Single(trackedTitles);
    }

    [Fact]
    public async Task TrackMovieAsync_Rejects_Blank_Title()
    {
        var provider = new TestTitleTrackingProvider();
        var service = new TrackMovieService(provider, CreateNoOpRepository());

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

        var service = new TrackMovieService(provider, CreateNoOpRepository());

        await service.TrackMovieAsync("Dune Part Two");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackMovieAsync("Dune Part Two"));
    }

    [Fact]
    public async Task TrackMovieAsync_Saves_Tracked_Title()
    {
        var trackedTitle = new TrackedTitle(
            ProviderId: "trakt:movie:654321",
            Title: "Dune: Part Two",
            Type: TrackedTitleType.Movie,
            Platform: "Max");

        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackMovieAsyncHandler = (_, _, _) =>
                Task.FromResult(trackedTitle)
        };

        TrackedTitle? savedTitle = null;

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (title, _) =>
            {
                savedTitle = title;
                return Task.CompletedTask;
            }
        };

        var service = new TrackMovieService(provider, repository);

        await service.TrackMovieAsync("Dune: Part Two", "Max");

        Assert.Equal(trackedTitle, savedTitle);
    }

    [Fact]
    public async Task TrackMovieAsync_Does_Not_Save_When_Provider_Throws()
    {
        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackMovieAsyncHandler = (_, _, _) =>
                throw new InvalidOperationException("Boom")
        };

        var repositoryCalled = false;

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) =>
            {
                repositoryCalled = true;
                return Task.CompletedTask;
            }
        };

        var service = new TrackMovieService(provider, repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackMovieAsync("Dune: Part Two", "Max"));

        Assert.False(repositoryCalled);
    }

    [Fact]
    public async Task TrackMovieAsync_Saves_Exactly_Once()
    {
        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackMovieAsyncHandler = (_, _, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:movie:654321",
                    Title: "Dune: Part Two",
                    Type: TrackedTitleType.Movie,
                    Platform: "Max"))
        };

        var saveCount = 0;

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) =>
            {
                saveCount++;
                return Task.CompletedTask;
            }
        };

        var service = new TrackMovieService(provider, repository);

        await service.TrackMovieAsync("Dune: Part Two", "Max");

        Assert.Equal(1, saveCount);
    }

    [Fact]
    public async Task TrackMovieAsync_Trims_Platform()
    {
        TrackedTitle? savedTitle = null;

        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackMovieAsyncHandler = (title, platform, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:movie:654321",
                    Title: title,
                    Type: TrackedTitleType.Movie,
                    Platform: platform))
        };

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (title, _) =>
            {
                savedTitle = title;
                return Task.CompletedTask;
            }
        };

        var service = new TrackMovieService(provider, repository);

        await service.TrackMovieAsync("Dune: Part Two", "  Max  ");

        Assert.NotNull(savedTitle);
        Assert.Equal("Max", savedTitle!.Platform);
    }

    private static TestTrackedTitleRepository CreateNoOpRepository()
    {
        return new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) => Task.CompletedTask
        };
    }
}
