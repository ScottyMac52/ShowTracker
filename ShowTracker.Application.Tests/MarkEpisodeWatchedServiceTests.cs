using ShowTracker.Application;
using ShowTracker.Domain;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Tests;

public sealed class MarkEpisodeWatchedServiceTests
{
    [Fact]
    public async Task MarkEpisodeWatchedAsync_Calls_Provider()
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new MarkEpisodeWatchedService(provider);

        await service.MarkEpisodeWatchedAsync("Andor", 2, 5);

        Assert.Equal("Andor", provider.ShowTitle);
        Assert.Equal(2, provider.SeasonNumber);
        Assert.Equal(5, provider.EpisodeNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MarkEpisodeWatchedAsync_Rejects_Blank_Show_Name(
        string showTitle)
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new MarkEpisodeWatchedService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.MarkEpisodeWatchedAsync(showTitle, 2, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task MarkEpisodeWatchedAsync_Rejects_Invalid_Season(
        int seasonNumber)
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new MarkEpisodeWatchedService(provider);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.MarkEpisodeWatchedAsync("Andor", seasonNumber, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task MarkEpisodeWatchedAsync_Rejects_Invalid_Episode(
        int episodeNumber)
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new MarkEpisodeWatchedService(provider);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.MarkEpisodeWatchedAsync("Andor", 2, episodeNumber));
    }

    private sealed class FakeWatchTrackingProvider : IWatchTrackingProvider
    {
        public string? ShowTitle { get; private set; }
        public int SeasonNumber { get; private set; }
        public int EpisodeNumber { get; private set; }

        public Task MarkEpisodeWatchedAsync(
            string showTitle,
            int seasonNumber,
            int episodeNumber,
            CancellationToken cancellationToken = default)
        {
            ShowTitle = showTitle;
            SeasonNumber = seasonNumber;
            EpisodeNumber = episodeNumber;

            return Task.CompletedTask;
        }

        public Task MarkMovieWatchedAsync(
            string movieTitle,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<WatchProgress?> GetShowProgressAsync(
            string showTitle,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}