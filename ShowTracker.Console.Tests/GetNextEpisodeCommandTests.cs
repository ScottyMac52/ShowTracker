using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class GetNextEpisodeCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Returns_Next_Episode()
    {
        var service = new TestGetNextEpisodeToWatchService
        {
            GetNextEpisodeToWatchAsyncHandler = (_, _) =>
                Task.FromResult<NextEpisodeToWatch?>(
                    new("trakt:show:12345", "Andor", 2, 6, "What a Festive Evening"))
        };

        var command = new GetNextEpisodeCommand(service);

        var output = await command.ExecuteAsync(["next-episode", "Andor"]);

        Assert.Contains("Next episode", output);
        Assert.Contains("Andor", output);
        Assert.Contains("S2E6", output);
        Assert.Contains("What a Festive Evening", output);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Message_When_No_Next_Episode_Found()
    {
        var service = new TestGetNextEpisodeToWatchService
        {
            GetNextEpisodeToWatchAsyncHandler = (_, _) =>
                Task.FromResult<NextEpisodeToWatch?>(null)
        };

        var command = new GetNextEpisodeCommand(service);

        var output = await command.ExecuteAsync(["next-episode", "Andor"]);

        Assert.Equal("No next episode found.", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Title()
    {
        var command = new GetNextEpisodeCommand(new TestGetNextEpisodeToWatchService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["next-episode"]));
    }

    private sealed class TestGetNextEpisodeToWatchService : IGetNextEpisodeToWatchService
    {
        public Func<string, CancellationToken, Task<NextEpisodeToWatch?>>? GetNextEpisodeToWatchAsyncHandler { get; set; }

        public Task<NextEpisodeToWatch?> GetNextEpisodeToWatchAsync(
            string showTitle,
            CancellationToken cancellationToken = default)
        {
            if (GetNextEpisodeToWatchAsyncHandler is null)
                throw new NotImplementedException();

            return GetNextEpisodeToWatchAsyncHandler(showTitle, cancellationToken);
        }
    }
}