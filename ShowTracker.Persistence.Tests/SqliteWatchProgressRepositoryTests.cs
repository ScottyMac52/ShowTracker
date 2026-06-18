using Microsoft.Data.Sqlite;
using ShowTracker.Domain.Models;
using ShowTracker.Persistence;

namespace ShowTracker.Persistence.Tests;

public sealed class SqliteWatchProgressRepositoryTests
{
    [Fact]
    public async Task SaveAsync_Saves_Watch_Progress()
    {
        var repository = CreateRepository();

        var progress = CreateProgress();

        await repository.SaveAsync(progress);

        var result = await repository.GetAsync("Andor");

        Assert.NotNull(result);
        Assert.Equal(progress, result);
    }

    [Fact]
    public async Task GetAsync_Returns_Null_When_Missing()
    {
        var repository = CreateRepository();

        var result = await repository.GetAsync("Missing Show");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetAsync_Rejects_Blank_Show_Title(string showTitle)
    {
        var repository = CreateRepository();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            repository.GetAsync(showTitle));
    }

    [Fact]
    public async Task SaveAsync_Rejects_Null_Progress()
    {
        var repository = CreateRepository();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            repository.SaveAsync(null!));
    }

    private static SqliteWatchProgressRepository CreateRepository()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        return new SqliteWatchProgressRepository(connection);
    }

    private static WatchProgress CreateProgress()
    {
        return new WatchProgress(
            ProviderId: "trakt:show:12345",
            ShowTitle: "Andor",
            LastWatchedSeason: 2,
            LastWatchedEpisode: 5,
            LastWatchedEpisodeTitle: "Messenger",
            NextSeason: 2,
            NextEpisode: 6,
            NextEpisodeTitle: "What a Festive Evening");
    }
}