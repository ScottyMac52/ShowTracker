using ShowTracker.Application;
using ShowTracker.Domain;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Tests;

public sealed class GetShowProgressServiceTests
{
    [Fact]
    public async Task GetShowProgressAsync_Returns_Progress_From_Provider()
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new GetShowProgressService(provider);

        var progress = await service.GetShowProgressAsync("Andor");

        Assert.NotNull(progress);
        Assert.Equal("Andor", progress!.ShowTitle);
        Assert.Equal(2, progress.LastWatchedSeason);
        Assert.Equal(5, progress.LastWatchedEpisode);
        Assert.Equal(2, progress.NextSeason);
        Assert.Equal(6, progress.NextEpisode);
    }

    [Fact]
    public async Task GetShowProgressAsync_Returns_Null_When_Show_Is_Unknown()
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new GetShowProgressService(provider);

        var progress = await service.GetShowProgressAsync("Unknown Show");

        Assert.Null(progress);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetShowProgressAsync_Rejects_Blank_Show_Title(string showTitle)
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new GetShowProgressService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetShowProgressAsync(showTitle));
    }

    [Fact]
    public async Task GetShowProgressAsync_Trims_Show_Title()
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new GetShowProgressService(provider);

        await service.GetShowProgressAsync("  Andor  ");

        Assert.Equal("Andor", provider.RequestedShowTitle);
    }

    private sealed class FakeWatchTrackingProvider : IWatchTrackingProvider
    {
        public string? RequestedShowTitle { get; private set; }

        public Task<WatchProgress?> GetShowProgressAsync(
            string showTitle,
            CancellationToken cancellationToken = default)
        {
            RequestedShowTitle = showTitle;

            if (!string.Equals(showTitle, "Andor", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult<WatchProgress?>(null);

            return Task.FromResult<WatchProgress?>(
                new WatchProgress(
                    ProviderId: "fake:show:andor",
                    ShowTitle: "Andor",
                    LastWatchedSeason: 2,
                    LastWatchedEpisode: 5,
                    LastWatchedEpisodeTitle: "Messenger",
                    NextSeason: 2,
                    NextEpisode: 6,
                    NextEpisodeTitle: "What a Festive Evening"));
        }

        public Task MarkMovieWatchedAsync(
            string movieTitle,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task MarkEpisodeWatchedAsync(
            string showTitle,
            int seasonNumber,
            int episodeNumber,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}