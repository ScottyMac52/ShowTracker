using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class MarkEpisodeWatchedServiceTests
{
    [Fact]
    public async Task MarkEpisodeWatchedAsync_Calls_Provider()
    {
        string? capturedShowTitle = null;
        int capturedSeasonNumber = 0;
        int capturedEpisodeNumber = 0;

        var provider = new TestWatchTrackingProvider
        {
            MarkEpisodeWatchedAsyncHandler = (showTitle, seasonNumber, episodeNumber, _) =>
            {
                capturedShowTitle = showTitle;
                capturedSeasonNumber = seasonNumber;
                capturedEpisodeNumber = episodeNumber;
                return Task.CompletedTask;
            }
        };

        var service = new MarkEpisodeWatchedService(provider);

        await service.MarkEpisodeWatchedAsync("Andor", 2, 5);

        Assert.Equal("Andor", capturedShowTitle);
        Assert.Equal(2, capturedSeasonNumber);
        Assert.Equal(5, capturedEpisodeNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MarkEpisodeWatchedAsync_Rejects_Blank_Show_Name(string showTitle)
    {
        var provider = new TestWatchTrackingProvider();
        var service = new MarkEpisodeWatchedService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.MarkEpisodeWatchedAsync(showTitle, 2, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task MarkEpisodeWatchedAsync_Rejects_Invalid_Season(int seasonNumber)
    {
        var provider = new TestWatchTrackingProvider();
        var service = new MarkEpisodeWatchedService(provider);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.MarkEpisodeWatchedAsync("Andor", seasonNumber, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task MarkEpisodeWatchedAsync_Rejects_Invalid_Episode(int episodeNumber)
    {
        var provider = new TestWatchTrackingProvider();
        var service = new MarkEpisodeWatchedService(provider);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.MarkEpisodeWatchedAsync("Andor", 2, episodeNumber));
    }
}