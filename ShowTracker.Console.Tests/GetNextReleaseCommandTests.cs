using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class GetNextReleaseCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Returns_No_Upcoming_Release_When_Service_Returns_Null()
    {
        var service = new TestGetNextReleaseService
        {
            GetNextReleaseAsyncHandler = (_, _) =>
                Task.FromResult<UpcomingRelease?>(null)
        };

        var command = new GetNextReleaseCommand(service);

        var result = await command.ExecuteAsync(["next-release", "The Boys"]);

        Assert.Equal("No upcoming release found for: The Boys", result);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Show_Release_With_Episode_Number_And_Title()
    {
        var service = new TestGetNextReleaseService
        {
            GetNextReleaseAsyncHandler = (_, _) =>
                Task.FromResult<UpcomingRelease?>(
                    new UpcomingRelease(
                        ProviderId: "139960",
                        Title: "The Boys",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 6, 20),
                        SeasonNumber: 5,
                        EpisodeNumber: 1,
                        EpisodeTitle: "Episode One"))
        };

        var command = new GetNextReleaseCommand(service);

        var result = await command.ExecuteAsync(["next-release", "The Boys"]);

        Assert.Equal("2026-06-20: The Boys - S05E01 - Episode One", result);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Show_Release_With_Episode_Number_When_Episode_Title_Is_Missing()
    {
        var service = new TestGetNextReleaseService
        {
            GetNextReleaseAsyncHandler = (_, _) =>
                Task.FromResult<UpcomingRelease?>(
                    new UpcomingRelease(
                        ProviderId: "139960",
                        Title: "The Boys",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 6, 20),
                        SeasonNumber: 5,
                        EpisodeNumber: 1))
        };

        var command = new GetNextReleaseCommand(service);

        var result = await command.ExecuteAsync(["next-release", "The Boys"]);

        Assert.Equal("2026-06-20: The Boys - S05E01", result);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Movie_Release_Without_Episode_Metadata()
    {
        var service = new TestGetNextReleaseService
        {
            GetNextReleaseAsyncHandler = (_, _) =>
                Task.FromResult<UpcomingRelease?>(
                    new UpcomingRelease(
                        ProviderId: "987654",
                        Title: "Dune: Part Three",
                        Type: TrackedTitleType.Movie,
                        ReleaseDate: new DateOnly(2026, 6, 21)))
        };

        var command = new GetNextReleaseCommand(service);

        var result = await command.ExecuteAsync(["next-release", "Dune: Part Three"]);

        Assert.Equal("2026-06-21: Dune: Part Three", result);
    }

    [Fact]
    public async Task ExecuteAsync_Passes_Title_To_Service()
    {
        string? capturedTitle = null;

        var service = new TestGetNextReleaseService
        {
            GetNextReleaseAsyncHandler = (title, _) =>
            {
                capturedTitle = title;

                return Task.FromResult<UpcomingRelease?>(
                    new UpcomingRelease(
                        ProviderId: "139960",
                        Title: "The Boys",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 6, 20)));
            }
        };

        var command = new GetNextReleaseCommand(service);

        await command.ExecuteAsync(["next-release", "The Boys"]);

        Assert.Equal("The Boys", capturedTitle);
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
            string title,
            CancellationToken cancellationToken = default)
        {
            if (GetNextReleaseAsyncHandler is null)
                throw new NotImplementedException();

            return GetNextReleaseAsyncHandler(title, cancellationToken);
        }
    }
}