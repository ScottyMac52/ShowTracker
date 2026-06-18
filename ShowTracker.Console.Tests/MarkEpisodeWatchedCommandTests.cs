using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;

namespace ShowTracker.Console.Tests;

public sealed class MarkEpisodeWatchedCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Marks_Episode_Watched()
    {
        string? requestedTitle = null;
        int requestedSeason = 0;
        int requestedEpisode = 0;

        var service = new TestMarkEpisodeWatchedService
        {
            MarkEpisodeWatchedAsyncHandler = (title, season, episode, _) =>
            {
                requestedTitle = title;
                requestedSeason = season;
                requestedEpisode = episode;
                return Task.CompletedTask;
            }
        };

        var command = new MarkEpisodeWatchedCommand(service);

        await command.ExecuteAsync(["watched-episode", "Andor", "2", "5"]);

        Assert.Equal("Andor", requestedTitle);
        Assert.Equal(2, requestedSeason);
        Assert.Equal(5, requestedEpisode);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Watched_Message()
    {
        var service = new TestMarkEpisodeWatchedService
        {
            MarkEpisodeWatchedAsyncHandler = (_, _, _, _) => Task.CompletedTask
        };

        var command = new MarkEpisodeWatchedCommand(service);

        var output = await command.ExecuteAsync(["watched-episode", "Andor", "2", "5"]);

        Assert.Contains("Marked watched", output);
        Assert.Contains("Andor", output);
        Assert.Contains("S2E5", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Arguments()
    {
        var command = new MarkEpisodeWatchedCommand(new TestMarkEpisodeWatchedService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["watched-episode", "Andor"]));
    }

    private sealed class TestMarkEpisodeWatchedService : IMarkEpisodeWatchedService
    {
        public Func<string, int, int, CancellationToken, Task>? MarkEpisodeWatchedAsyncHandler { get; set; }

        public Task MarkEpisodeWatchedAsync(
            string showTitle,
            int seasonNumber,
            int episodeNumber,
            CancellationToken cancellationToken = default)
        {
            if (MarkEpisodeWatchedAsyncHandler is null)
                throw new NotImplementedException();

            return MarkEpisodeWatchedAsyncHandler(showTitle, seasonNumber, episodeNumber, cancellationToken);
        }
    }
}