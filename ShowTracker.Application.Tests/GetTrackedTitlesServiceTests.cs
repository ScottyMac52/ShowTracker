using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class GetTrackedTitlesServiceTests
{
    [Fact]
    public async Task GetTrackedTitlesAsync_Returns_All_Tracked_Titles()
    {
        IReadOnlyList<TrackedTitle> trackedTitles =
        [
            new(
                ProviderId: "trakt:show:12345",
                Title: "Andor",
                Type: TrackedTitleType.Show,
                Platform: "Disney Plus"),

            new(
                ProviderId: "trakt:movie:654321",
                Title: "Dune: Part Two",
                Type: TrackedTitleType.Movie,
                Platform: "Max")
        ];

        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult(trackedTitles)
        };

        var service = new GetTrackedTitlesService(repository);

        var results = await service.GetTrackedTitlesAsync();

        Assert.Equal(2, results.Count);
        Assert.Equal(trackedTitles, results);
    }

    [Fact]
    public async Task GetTrackedTitlesAsync_Returns_Empty_List_When_No_Titles_Are_Tracked()
    {
        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>([])
        };

        var service = new GetTrackedTitlesService(repository);

        var results = await service.GetTrackedTitlesAsync();

        Assert.Empty(results);
    }
}