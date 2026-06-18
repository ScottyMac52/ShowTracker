using ShowTracker.Application.Services;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class TrackShowServiceTests
{
    [Fact]
    public async Task TrackShowAsync_Tracks_Show_Through_Provider()
    {
        var trackedTitles = new List<TrackedTitle>();

        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackShowAsyncHandler = (showTitle, platform, _) =>
            {
                var trackedTitle = new TrackedTitle(
                    ProviderId: $"fake:show:{showTitle}",
                    Title: showTitle,
                    Type: TrackedTitleType.Show,
                    Platform: platform);

                trackedTitles.Add(trackedTitle);

                return Task.FromResult(trackedTitle);
            }
        };

        var service = new TrackShowService(provider, CreateNoOpRepository());
        var result = await service.TrackShowAsync("Your Friends & Neighbors", "Apple TV");

        Assert.Equal("Your Friends & Neighbors", result.Title);
        Assert.Equal(TrackedTitleType.Show, result.Type);
        Assert.Equal("Apple TV", result.Platform);
        Assert.Single(trackedTitles);
    }

    [Fact]
    public async Task TrackShowAsync_Rejects_Blank_Title()
    {
        var provider = new TestTitleTrackingProvider();
        var service = new TrackShowService(
                            provider,
                            new TestTrackedTitleRepository());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.TrackShowAsync("   ", "Apple TV"));
    }

    [Fact]
    public async Task TrackShowAsync_Does_Not_Allow_Duplicate_Shows()
    {
        var trackedTitles = new List<TrackedTitle>();

        var provider = CreateProviderBackedBy(trackedTitles);
        var service = new TrackShowService(provider, CreateNoOpRepository());
        await service.TrackShowAsync("Andor");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackShowAsync("Andor"));
    }

    [Fact]
    public async Task TrackShowAsync_Does_Not_Allow_Duplicate_Shows_Ignoring_Case()
    {
        var trackedTitles = new List<TrackedTitle>();

        var provider = CreateProviderBackedBy(trackedTitles);
        var service = new TrackShowService(provider, CreateNoOpRepository());
        await service.TrackShowAsync("Andor");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackShowAsync("andor"));
    }

    [Fact]
    public async Task TrackShowAsync_Saves_Tracked_Title()
    {
        var trackedTitle = new TrackedTitle(
            ProviderId: "trakt:show:12345",
            Title: "Andor",
            Type: TrackedTitleType.Show,
            Platform: "Disney Plus");

        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackShowAsyncHandler = (_, _, _) =>
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

        var service = new TrackShowService(provider, repository);

        await service.TrackShowAsync("Andor", "Disney Plus");

        Assert.Equal(trackedTitle, savedTitle);
    }

    [Fact]
    public async Task TrackShowAsync_Does_Not_Save_When_Provider_Throws()
    {
        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackShowAsyncHandler = (_, _, _) =>
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

        var service = new TrackShowService(provider, repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackShowAsync("Andor", "Disney Plus"));

        Assert.False(repositoryCalled);
    }

    [Fact]
    public async Task TrackShowAsync_Saves_Exactly_Once()
    {
        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackShowAsyncHandler = (_, _, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:show:12345",
                    Title: "Andor",
                    Type: TrackedTitleType.Show,
                    Platform: "Disney Plus"))
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

        var service = new TrackShowService(provider, repository);

        await service.TrackShowAsync("Andor", "Disney Plus");

        Assert.Equal(1, saveCount);
    }

    [Fact]
    public async Task TrackShowAsync_Trims_Platform()
    {
        TrackedTitle? savedTitle = null;

        var provider = new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (_, _) =>
                Task.FromResult<TrackedTitle?>(null),

            TrackShowAsyncHandler = (title, platform, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:show:12345",
                    Title: title,
                    Type: TrackedTitleType.Show,
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

        var service = new TrackShowService(provider, repository);

        await service.TrackShowAsync("Andor", "  Disney Plus  ");

        Assert.NotNull(savedTitle);
        Assert.Equal("Disney Plus", savedTitle!.Platform);
    }

    private static TestTrackedTitleRepository CreateNoOpRepository()
    {
        return new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) => Task.CompletedTask
        };
    }

    private static TestTitleTrackingProvider CreateProviderBackedBy(
        List<TrackedTitle> trackedTitles)
    {
        return new TestTitleTrackingProvider
        {
            FindTrackedTitleAsyncHandler = (title, _) =>
            {
                var trackedTitle = trackedTitles.SingleOrDefault(t =>
                    string.Equals(t.Title, title, StringComparison.OrdinalIgnoreCase));

                return Task.FromResult(trackedTitle);
            },

            TrackShowAsyncHandler = (showTitle, platform, _) =>
            {
                var trackedTitle = new TrackedTitle(
                    ProviderId: $"fake:show:{showTitle}",
                    Title: showTitle,
                    Type: TrackedTitleType.Show,
                    Platform: platform);

                trackedTitles.Add(trackedTitle);

                return Task.FromResult(trackedTitle);
            }
        };
    }
}
