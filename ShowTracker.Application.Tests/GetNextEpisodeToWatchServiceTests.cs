using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class GetNextEpisodeToWatchServiceTests
{
    [Fact]
    public async Task GetNextEpisodeToWatchAsync_Returns_Next_Episode_From_Progress()
    {
        var provider = new TestWatchTrackingProvider
        {
            GetShowProgressAsyncHandler = (_, _) =>
                Task.FromResult<WatchProgress?>(
                    new WatchProgress(
                        ProviderId: "fake:show:andor",
                        ShowTitle: "Andor",
                        LastWatchedSeason: 2,
                        LastWatchedEpisode: 5,
                        LastWatchedEpisodeTitle: "Messenger",
                        NextSeason: 2,
                        NextEpisode: 6,
                        NextEpisodeTitle: "What a Festive Evening"))
        };

        var service = new GetNextEpisodeToWatchService(provider);

        var result = await service.GetNextEpisodeToWatchAsync("Andor");

        Assert.NotNull(result);
        Assert.Equal("Andor", result!.ShowTitle);
        Assert.Equal(2, result.SeasonNumber);
        Assert.Equal(6, result.EpisodeNumber);
        Assert.Equal("What a Festive Evening", result.EpisodeTitle);
    }

    [Fact]
    public async Task GetNextEpisodeToWatchAsync_Returns_Null_When_Show_Is_Unknown()
    {
        var provider = new TestWatchTrackingProvider
        {
            GetShowProgressAsyncHandler = (_, _) =>
                Task.FromResult<WatchProgress?>(null)
        };

        var service = new GetNextEpisodeToWatchService(provider);

        var result = await service.GetNextEpisodeToWatchAsync("Unknown Show");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetNextEpisodeToWatchAsync_Returns_Null_When_Progress_Has_No_Next_Episode()
    {
        var provider = new TestWatchTrackingProvider
        {
            GetShowProgressAsyncHandler = (_, _) =>
                Task.FromResult<WatchProgress?>(
                    new WatchProgress(
                        ProviderId: "fake:show:andor",
                        ShowTitle: "Andor",
                        LastWatchedSeason: 2,
                        LastWatchedEpisode: 12,
                        LastWatchedEpisodeTitle: "Finale",
                        NextSeason: null,
                        NextEpisode: null,
                        NextEpisodeTitle: null))
        };

        var service = new GetNextEpisodeToWatchService(provider);

        var result = await service.GetNextEpisodeToWatchAsync("Andor");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetNextEpisodeToWatchAsync_Rejects_Blank_Show_Title(string showTitle)
    {
        var provider = new TestWatchTrackingProvider();
        var service = new GetNextEpisodeToWatchService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetNextEpisodeToWatchAsync(showTitle));
    }

    [Fact]
    public async Task GetNextEpisodeToWatchAsync_Trims_Show_Title()
    {
        string? requestedShowTitle = null;

        var provider = new TestWatchTrackingProvider
        {
            GetShowProgressAsyncHandler = (showTitle, _) =>
            {
                requestedShowTitle = showTitle;
                return Task.FromResult<WatchProgress?>(null);
            }
        };

        var service = new GetNextEpisodeToWatchService(provider);

        await service.GetNextEpisodeToWatchAsync("  Andor  ");

        Assert.Equal("Andor", requestedShowTitle);
    }
}