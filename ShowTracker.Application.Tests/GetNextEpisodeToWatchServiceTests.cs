using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Tests;

public sealed class GetNextEpisodeToWatchServiceTests
{
    [Fact]
    public async Task GetNextEpisodeToWatchAsync_Returns_Next_Episode_From_Progress()
    {
        var provider = new FakeWatchTrackingProvider();
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
        var provider = new FakeWatchTrackingProvider();
        var service = new GetNextEpisodeToWatchService(provider);

        var result = await service.GetNextEpisodeToWatchAsync("Unknown Show");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetNextEpisodeToWatchAsync_Returns_Null_When_Progress_Has_No_Next_Episode()
    {
        var provider = new FakeWatchTrackingProvider(
            new WatchProgress(
                ProviderId: "fake:show:andor",
                ShowTitle: "Andor",
                LastWatchedSeason: 2,
                LastWatchedEpisode: 12,
                LastWatchedEpisodeTitle: "Finale",
                NextSeason: null,
                NextEpisode: null,
                NextEpisodeTitle: null));

        var service = new GetNextEpisodeToWatchService(provider);

        var result = await service.GetNextEpisodeToWatchAsync("Andor");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetNextEpisodeToWatchAsync_Rejects_Blank_Show_Title(string showTitle)
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new GetNextEpisodeToWatchService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetNextEpisodeToWatchAsync(showTitle));
    }

    [Fact]
    public async Task GetNextEpisodeToWatchAsync_Trims_Show_Title()
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new GetNextEpisodeToWatchService(provider);

        await service.GetNextEpisodeToWatchAsync("  Andor  ");

        Assert.Equal("Andor", provider.RequestedShowTitle);
    }

    private sealed class FakeWatchTrackingProvider : IWatchTrackingProvider
    {
        private readonly WatchProgress? _progress;

        public FakeWatchTrackingProvider()
            : this(new WatchProgress(
                ProviderId: "fake:show:andor",
                ShowTitle: "Andor",
                LastWatchedSeason: 2,
                LastWatchedEpisode: 5,
                LastWatchedEpisodeTitle: "Messenger",
                NextSeason: 2,
                NextEpisode: 6,
                NextEpisodeTitle: "What a Festive Evening"))
        {
        }

        public FakeWatchTrackingProvider(WatchProgress? progress)
        {
            _progress = progress;
        }

        public string? RequestedShowTitle { get; private set; }

        public Task<WatchProgress?> GetShowProgressAsync(
            string showTitle,
            CancellationToken cancellationToken = default)
        {
            RequestedShowTitle = showTitle;

            if (!string.Equals(showTitle, "Andor", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult<WatchProgress?>(null);

            return Task.FromResult(_progress);
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