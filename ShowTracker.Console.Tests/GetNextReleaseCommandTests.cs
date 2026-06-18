using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class GetNextReleaseCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Returns_Next_Release()
    {
        var service = new TestGetNextReleaseService
        {
            GetNextReleaseAsyncHandler = (_, _) =>
                Task.FromResult<UpcomingRelease?>(
                    new(
                        ProviderId: "trakt:show:12345",
                        Title: "Andor",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 7, 10),
                        SeasonNumber: 2,
                        EpisodeNumber: 1,
                        EpisodeTitle: "Episode One",
                        Platform: "Disney Plus"))
        };

        var command = new GetNextReleaseCommand(service);

        var output = await command.ExecuteAsync(["next-release", "Andor"]);

        Assert.Contains("Next release", output);
        Assert.Contains("Andor", output);
        Assert.Contains("2026-07-10", output);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Message_When_No_Next_Release_Found()
    {
        var service = new TestGetNextReleaseService
        {
            GetNextReleaseAsyncHandler = (_, _) =>
                Task.FromResult<UpcomingRelease?>(null)
        };

        var command = new GetNextReleaseCommand(service);

        var output = await command.ExecuteAsync(["next-release", "Andor"]);

        Assert.Equal("No next release found.", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Title()
    {
        var command = new GetNextReleaseCommand(new TestGetNextReleaseService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["next-release"]));
    }

    private sealed class TestGetNextReleaseService : IGetNextReleaseService
    {
        public Func<string, CancellationToken, Task<UpcomingRelease?>>? GetNextReleaseAsyncHandler { get; set; }

        public Task<UpcomingRelease?> GetNextReleaseAsync(
            string showTitle,
            CancellationToken cancellationToken = default)
        {
            if (GetNextReleaseAsyncHandler is null)
                throw new NotImplementedException();

            return GetNextReleaseAsyncHandler(showTitle, cancellationToken);
        }
    }
}