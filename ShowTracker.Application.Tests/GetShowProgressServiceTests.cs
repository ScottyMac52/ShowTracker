using ShowTracker.Application.Services;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class GetShowProgressServiceTests
{
    [Fact]
    public async Task GetShowProgressAsync_Returns_Progress_From_Repository()
    {
        var expected = new WatchProgress(
            ProviderId: "trakt:show:12345",
            ShowTitle: "Andor",
            LastWatchedSeason: 2,
            LastWatchedEpisode: 5,
            LastWatchedEpisodeTitle: "Messenger",
            NextSeason: 2,
            NextEpisode: 6,
            NextEpisodeTitle: "What a Festive Evening");

        var repository = new TestWatchProgressRepository
        {
            GetAsyncHandler = (_, _) =>
                Task.FromResult<WatchProgress?>(expected)
        };

        var service = new GetShowProgressService(repository);

        var progress = await service.GetShowProgressAsync("Andor");

        Assert.Equal(expected, progress);
    }

    [Fact]
    public async Task GetShowProgressAsync_Returns_Null_When_Show_Is_Unknown()
    {
        var repository = new TestWatchProgressRepository
        {
            GetAsyncHandler = (_, _) =>
                Task.FromResult<WatchProgress?>(null)
        };

        var service = new GetShowProgressService(repository);

        var progress = await service.GetShowProgressAsync("Unknown Show");

        Assert.Null(progress);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetShowProgressAsync_Rejects_Blank_Show_Title(string showTitle)
    {
        var repository = new TestWatchProgressRepository();
        var service = new GetShowProgressService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetShowProgressAsync(showTitle));
    }

    [Fact]
    public async Task GetShowProgressAsync_Trims_Show_Title()
    {
        string? requestedShowTitle = null;

        var repository = new TestWatchProgressRepository
        {
            GetAsyncHandler = (showTitle, _) =>
            {
                requestedShowTitle = showTitle;
                return Task.FromResult<WatchProgress?>(null);
            }
        };

        var service = new GetShowProgressService(repository);

        await service.GetShowProgressAsync("  Andor  ");

        Assert.Equal("Andor", requestedShowTitle);
    }
}