using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Domain.Tests;

public sealed class ProviderInterfaceContractTests
{
    [Fact]
    public async Task TitleTrackingProvider_Can_Track_Show()
    {
        var provider = new TestTitleTrackingProvider
        {
            TrackShowAsyncHandler = (showTitle, platform, _) =>
                Task.FromResult(
                    new TrackedTitle(
                        ProviderId: $"fake:show:{showTitle}",
                        Title: showTitle,
                        Type: TrackedTitleType.Show,
                        Platform: platform))
        };

        var result = await provider.TrackShowAsync("Your Friends & Neighbors", "Apple TV");

        Assert.Equal("Your Friends & Neighbors", result.Title);
        Assert.Equal(TrackedTitleType.Show, result.Type);
        Assert.Equal("Apple TV", result.Platform);
    }

    [Fact]
    public async Task WatchTrackingProvider_Can_Mark_Episode_Watched()
    {
        WatchProgress? storedProgress = null;

        var provider = new TestWatchTrackingProvider
        {
            MarkEpisodeWatchedAsyncHandler = (showTitle, seasonNumber, episodeNumber, _) =>
            {
                storedProgress = new WatchProgress(
                    ProviderId: $"fake:show:{showTitle}",
                    ShowTitle: showTitle,
                    LastWatchedSeason: seasonNumber,
                    LastWatchedEpisode: episodeNumber,
                    LastWatchedEpisodeTitle: null,
                    NextSeason: null,
                    NextEpisode: null,
                    NextEpisodeTitle: null);

                return Task.CompletedTask;
            },

            GetShowProgressAsyncHandler = (_, _) =>
                Task.FromResult(storedProgress)
        };

        await provider.MarkEpisodeWatchedAsync("Andor", 2, 5);

        var progress = await provider.GetShowProgressAsync("Andor");

        Assert.NotNull(progress);
        Assert.Equal(2, progress!.LastWatchedSeason);
        Assert.Equal(5, progress.LastWatchedEpisode);
    }
}