using ShowTracker.Application.Services;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class TrackMovieServiceTests
{
    [Fact]
    public async Task TrackMovieAsync_Tracks_Movie_Through_Provider()
    {
        var provider = new TestTitleTrackingProvider
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:movie:654321",
                    Title: title,
                    Type: TrackedTitleType.Movie,
                    Platform: platform))
        };

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) => Task.CompletedTask
        };

        var service = new TrackMovieService(provider, repository);

        var result = await service.TrackMovieAsync("Dune: Part Two");

        Assert.Equal("Dune: Part Two", result.Title);
        Assert.Equal(TrackedTitleType.Movie, result.Type);
    }

    [Fact]
    public async Task TrackMovieAsync_Does_Not_Allow_Duplicate_Movies()
    {
        var provider = new TestTitleTrackingProvider();

        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new(
                        ProviderId: "trakt:movie:654321",
                        Title: "Dune: Part Two",
                        Type: TrackedTitleType.Movie)
                ])
        };

        var service = new TrackMovieService(provider, repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackMovieAsync("Dune: Part Two"));
    }

    [Fact]
    public async Task TrackMovieAsync_Saves_Tracked_Title()
    {
        TrackedTitle? savedTitle = null;

        var provider = new TestTitleTrackingProvider
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:movie:654321",
                    Title: title,
                    Type: TrackedTitleType.Movie,
                    Platform: platform))
        };

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (trackedTitle, _) =>
            {
                savedTitle = trackedTitle;
                return Task.CompletedTask;
            }
        };

        var service = new TrackMovieService(provider, repository);

        await service.TrackMovieAsync("Dune: Part Two");

        Assert.NotNull(savedTitle);
        Assert.Equal("Dune: Part Two", savedTitle!.Title);
        Assert.Equal("trakt:movie:654321", savedTitle.ProviderId);
        Assert.Equal(TrackedTitleType.Movie, savedTitle.Type);
    }

    [Fact]
    public async Task TrackMovieAsync_Does_Not_Save_When_Provider_Throws()
    {
        var saveCalled = false;

        var provider = new TestTitleTrackingProvider
        {
            TrackMovieAsyncHandler = (_, _, _) =>
                throw new InvalidOperationException("Provider failed.")
        };

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) =>
            {
                saveCalled = true;
                return Task.CompletedTask;
            }
        };

        var service = new TrackMovieService(provider, repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackMovieAsync("Dune: Part Two"));

        Assert.False(saveCalled);
    }

    [Fact]
    public async Task TrackMovieAsync_Saves_Exactly_Once()
    {
        var saveCount = 0;

        var provider = new TestTitleTrackingProvider
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:movie:654321",
                    Title: title,
                    Type: TrackedTitleType.Movie,
                    Platform: platform))
        };

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) =>
            {
                saveCount++;
                return Task.CompletedTask;
            }
        };

        var service = new TrackMovieService(provider, repository);

        await service.TrackMovieAsync("Dune: Part Two");

        Assert.Equal(1, saveCount);
    }

    [Fact]
    public async Task TrackMovieAsync_Trims_Platform()
    {
        string? requestedPlatform = null;

        var provider = new TestTitleTrackingProvider
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
            {
                requestedPlatform = platform;

                return Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:movie:654321",
                    Title: title,
                    Type: TrackedTitleType.Movie,
                    Platform: platform));
            }
        };

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) => Task.CompletedTask
        };

        var service = new TrackMovieService(provider, repository);

        await service.TrackMovieAsync("Dune: Part Two", "  Max  ");

        Assert.Equal("Max", requestedPlatform);
    }
}