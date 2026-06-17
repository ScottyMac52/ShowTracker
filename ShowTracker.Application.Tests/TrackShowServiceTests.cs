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

        var service = new TrackShowService(provider);

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
        var service = new TrackShowService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.TrackShowAsync("   ", "Apple TV"));
    }

    [Fact]
    public async Task TrackShowAsync_Does_Not_Allow_Duplicate_Shows()
    {
        var trackedTitles = new List<TrackedTitle>();

        var provider = CreateProviderBackedBy(trackedTitles);
        var service = new TrackShowService(provider);

        await service.TrackShowAsync("Andor");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackShowAsync("Andor"));
    }

    [Fact]
    public async Task TrackShowAsync_Does_Not_Allow_Duplicate_Shows_Ignoring_Case()
    {
        var trackedTitles = new List<TrackedTitle>();

        var provider = CreateProviderBackedBy(trackedTitles);
        var service = new TrackShowService(provider);

        await service.TrackShowAsync("Andor");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TrackShowAsync("andor"));
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
