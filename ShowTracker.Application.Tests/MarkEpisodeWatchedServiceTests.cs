using ShowTracker.Application.Services;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class MarkEpisodeWatchedServiceTests
{
    [Fact]
    public async Task MarkEpisodeWatchedAsync_Saves_Progress()
    {
        WatchProgress? savedProgress = null;

        var repository = new TestWatchProgressRepository
        {
            SaveAsyncHandler = (progress, _) =>
            {
                savedProgress = progress;
                return Task.CompletedTask;
            }
        };

        var service = new MarkEpisodeWatchedService(repository);

        await service.MarkEpisodeWatchedAsync("Andor", 2, 5);

        Assert.NotNull(savedProgress);
        Assert.Equal("Andor", savedProgress!.ShowTitle);
        Assert.Equal(2, savedProgress.LastWatchedSeason);
        Assert.Equal(5, savedProgress.LastWatchedEpisode);
    }

    [Fact]
    public async Task MarkEpisodeWatchedAsync_Trims_Show_Title()
    {
        WatchProgress? savedProgress = null;

        var repository = new TestWatchProgressRepository
        {
            SaveAsyncHandler = (progress, _) =>
            {
                savedProgress = progress;
                return Task.CompletedTask;
            }
        };

        var service = new MarkEpisodeWatchedService(repository);

        await service.MarkEpisodeWatchedAsync("  Andor  ", 2, 5);

        Assert.NotNull(savedProgress);
        Assert.Equal("Andor", savedProgress!.ShowTitle);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MarkEpisodeWatchedAsync_Rejects_Blank_Show_Name(string showTitle)
    {
        var repository = new TestWatchProgressRepository();
        var service = new MarkEpisodeWatchedService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.MarkEpisodeWatchedAsync(showTitle, 2, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task MarkEpisodeWatchedAsync_Rejects_Invalid_Season(int seasonNumber)
    {
        var repository = new TestWatchProgressRepository();
        var service = new MarkEpisodeWatchedService(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.MarkEpisodeWatchedAsync("Andor", seasonNumber, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task MarkEpisodeWatchedAsync_Rejects_Invalid_Episode(int episodeNumber)
    {
        var repository = new TestWatchProgressRepository();
        var service = new MarkEpisodeWatchedService(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.MarkEpisodeWatchedAsync("Andor", 2, episodeNumber));
    }
}