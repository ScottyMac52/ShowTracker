using ShowTracker.Application.Services;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class TrackShowServiceTests
{
    [Fact]
    public async Task TrackShowAsync_Tracks_Show_Through_Provider()
    {
        var provider = new TestTitleTrackingProvider
        {
            TrackShowAsyncHandler = (title, platform, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:show:12345",
                    Title: title,
                    Type: TrackedTitleType.Show,
                    Platform: platform))
        };

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) => Task.CompletedTask
        };

        var service = new TrackShowService(provider, repository);

        var result = await service.TrackShowAsync("Andor");

        Assert.Equal("Andor", result.Title);
        Assert.Equal(TrackedTitleType.Show, result.Type);
    }

    [Fact]
    public async Task TrackShowAsync_Does_Not_Allow_Duplicate_Shows()
    {
        var provider = new TestTitleTrackingProvider();

        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new(
                        ProviderId: "trakt:show:12345",
                        Title: "Andor",
                        Type: TrackedTitleType.Show)
                ])
        };

        var service = new TrackShowService(provider, repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackShowAsync("Andor"));
    }

    [Fact]
    public async Task TrackShowAsync_Does_Not_Allow_Duplicate_Shows_Ignoring_Case()
    {
        var provider = new TestTitleTrackingProvider();

        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new(
                        ProviderId: "trakt:show:12345",
                        Title: "ANDOR",
                        Type: TrackedTitleType.Show)
                ])
        };

        var service = new TrackShowService(provider, repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackShowAsync("andor"));
    }

    [Fact]
    public async Task TrackShowAsync_Saves_Tracked_Title()
    {
        TrackedTitle? savedTitle = null;

        var provider = new TestTitleTrackingProvider
        {
            TrackShowAsyncHandler = (title, platform, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:show:12345",
                    Title: title,
                    Type: TrackedTitleType.Show,
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

        var service = new TrackShowService(provider, repository);

        await service.TrackShowAsync("Andor");

        Assert.NotNull(savedTitle);
        Assert.Equal("Andor", savedTitle!.Title);
        Assert.Equal("trakt:show:12345", savedTitle.ProviderId);
        Assert.Equal(TrackedTitleType.Show, savedTitle.Type);
    }

    [Fact]
    public async Task TrackShowAsync_Does_Not_Save_When_Provider_Throws()
    {
        var saveCalled = false;

        var provider = new TestTitleTrackingProvider
        {
            TrackShowAsyncHandler = (_, _, _) =>
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

        var service = new TrackShowService(provider, repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackShowAsync("Andor"));

        Assert.False(saveCalled);
    }

    [Fact]
    public async Task TrackShowAsync_Saves_Exactly_Once()
    {
        var saveCount = 0;

        var provider = new TestTitleTrackingProvider
        {
            TrackShowAsyncHandler = (title, platform, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:show:12345",
                    Title: title,
                    Type: TrackedTitleType.Show,
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

        var service = new TrackShowService(provider, repository);

        await service.TrackShowAsync("Andor");

        Assert.Equal(1, saveCount);
    }

    [Fact]
    public async Task TrackShowAsync_Trims_Platform()
    {
        string? requestedPlatform = null;

        var provider = new TestTitleTrackingProvider
        {
            TrackShowAsyncHandler = (title, platform, _) =>
            {
                requestedPlatform = platform;

                return Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:show:12345",
                    Title: title,
                    Type: TrackedTitleType.Show,
                    Platform: platform));
            }
        };

        var repository = new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) => Task.CompletedTask
        };

        var service = new TrackShowService(provider, repository);

        await service.TrackShowAsync("Andor", "  Disney Plus  ");

        Assert.Equal("Disney Plus", requestedPlatform);
    }
}