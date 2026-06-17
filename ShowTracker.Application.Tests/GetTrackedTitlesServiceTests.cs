using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class GetTrackedTitlesServiceTests
{
    [Fact]
    public async Task GetTrackedTitlesAsync_Returns_All_Tracked_Titles()
    {
        var provider = new TestTitleTrackingProvider
        {
            GetTrackedTitlesAsyncHandler = _ =>
            {
                IReadOnlyList<TrackedTitle> titles =
                [
                    new(
                        ProviderId: "show-1",
                        Title: "Andor",
                        Type: TrackedTitleType.Show),

                    new(
                        ProviderId: "movie-1",
                        Title: "Dune Part Two",
                        Type: TrackedTitleType.Movie)
                ];

                return Task.FromResult(titles);
            }
        };

        var service = new GetTrackedTitlesService(provider);

        var results = await service.GetTrackedTitlesAsync();

        Assert.Equal(2, results.Count);

        Assert.Contains(results,
            t => t.Title == "Andor");

        Assert.Contains(results,
            t => t.Title == "Dune Part Two");
    }
}
