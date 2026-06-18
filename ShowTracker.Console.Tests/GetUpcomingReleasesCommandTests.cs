using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class GetUpcomingReleasesCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Returns_Upcoming_Releases()
    {
        var service = new TestGetUpcomingReleasesService
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>(
                [
                    new(
                        ProviderId: "trakt:show:12345",
                        Title: "Andor",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 7, 10),
                        SeasonNumber: 2,
                        EpisodeNumber: 1,
                        EpisodeTitle: "Episode One")
                ])
        };

        var command = new GetUpcomingReleasesCommand(service);

        var output = await command.ExecuteAsync(["releases"]);

        Assert.Contains("2026-07-10", output);
        Assert.Contains("Andor", output);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Message_When_No_Releases_Found()
    {
        var service = new TestGetUpcomingReleasesService
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>([])
        };

        var command = new GetUpcomingReleasesCommand(service);

        var output = await command.ExecuteAsync(["releases"]);

        Assert.Equal("No upcoming releases.", output);
    }

    private sealed class TestGetUpcomingReleasesService : IGetUpcomingReleasesService
    {
        public Func<CancellationToken, Task<IReadOnlyList<UpcomingRelease>>>? GetUpcomingReleasesAsyncHandler { get; set; }

        public Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
            CancellationToken cancellationToken = default)
        {
            if (GetUpcomingReleasesAsyncHandler is null)
                throw new NotImplementedException();

            return GetUpcomingReleasesAsyncHandler(cancellationToken);
        }
    }
}