using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class GetUpcomingReleasesCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Returns_No_Upcoming_Releases_When_Service_Returns_Empty_List()
    {
        var service = new TestGetUpcomingReleasesService
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>([])
        };

        var command = new GetUpcomingReleasesCommand(service);

        var result = await command.ExecuteAsync(["releases"]);

        Assert.Equal("No upcoming releases.", result);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Show_Release_With_Episode_Number_And_Title()
    {
        var service = new TestGetUpcomingReleasesService
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>(
                [
                    new UpcomingRelease(
                        ProviderId: "139960",
                        Title: "The Boys",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 6, 20),
                        SeasonNumber: 5,
                        EpisodeNumber: 1,
                        EpisodeTitle: "Episode One")
                ])
        };

        var command = new GetUpcomingReleasesCommand(service);

        var result = await command.ExecuteAsync(["releases"]);

        Assert.Equal("2026-06-20: The Boys - S05E01 - Episode One", result);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Show_Release_With_Episode_Number_When_Episode_Title_Is_Missing()
    {
        var service = new TestGetUpcomingReleasesService
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>(
                [
                    new UpcomingRelease(
                        ProviderId: "139960",
                        Title: "The Boys",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 6, 20),
                        SeasonNumber: 5,
                        EpisodeNumber: 1)
                ])
        };

        var command = new GetUpcomingReleasesCommand(service);

        var result = await command.ExecuteAsync(["releases"]);

        Assert.Equal("2026-06-20: The Boys - S05E01", result);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Movie_Release_Without_Episode_Metadata()
    {
        var service = new TestGetUpcomingReleasesService
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>(
                [
                    new UpcomingRelease(
                        ProviderId: "987654",
                        Title: "Dune: Part Three",
                        Type: TrackedTitleType.Movie,
                        ReleaseDate: new DateOnly(2026, 6, 21))
                ])
        };

        var command = new GetUpcomingReleasesCommand(service);

        var result = await command.ExecuteAsync(["releases"]);

        Assert.Equal("2026-06-21: Dune: Part Three", result);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Multiple_Releases_On_Separate_Lines()
    {
        var service = new TestGetUpcomingReleasesService
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>(
                [
                    new UpcomingRelease(
                        ProviderId: "139960",
                        Title: "The Boys",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 6, 20),
                        SeasonNumber: 5,
                        EpisodeNumber: 1,
                        EpisodeTitle: "Episode One"),
                    new UpcomingRelease(
                        ProviderId: "987654",
                        Title: "Dune: Part Three",
                        Type: TrackedTitleType.Movie,
                        ReleaseDate: new DateOnly(2026, 6, 21))
                ])
        };

        var command = new GetUpcomingReleasesCommand(service);

        var result = await command.ExecuteAsync(["releases"]);

        var expected = string.Join(
            Environment.NewLine,
            [
                "2026-06-20: The Boys - S05E01 - Episode One",
                "2026-06-21: Dune: Part Three"
            ]);

        Assert.Equal(expected, result);
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