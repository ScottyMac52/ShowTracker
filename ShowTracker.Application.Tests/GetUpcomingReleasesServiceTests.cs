using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class GetUpcomingReleasesServiceTests
{
    [Fact]
    public async Task GetUpcomingReleasesAsync_Returns_Releases_From_Provider()
    {
        var releases = CreateDefaultUpcomingReleases();

        var provider = new TestTitleTrackingProvider
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>(releases)
        };

        var service = new GetUpcomingReleasesService(provider);

        var results = await service.GetUpcomingReleasesAsync();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Title == "Andor" && r.SeasonNumber == 2);
        Assert.Contains(results, r => r.Title == "Dune Part Three");
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Returns_Empty_List_When_Provider_Has_None()
    {
        var provider = new TestTitleTrackingProvider
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>([])
        };

        var service = new GetUpcomingReleasesService(provider);

        var results = await service.GetUpcomingReleasesAsync();

        Assert.Empty(results);
    }

    private static IReadOnlyList<UpcomingRelease> CreateDefaultUpcomingReleases()
    {
        return
        [
            new(
                ProviderId: "release-1",
                Title: "Andor",
                Type: TrackedTitleType.Show,
                ReleaseDate: new DateOnly(2026, 7, 10),
                SeasonNumber: 2,
                EpisodeNumber: 1,
                EpisodeTitle: "Episode One",
                Platform: "Disney Plus"),

            new(
                ProviderId: "release-2",
                Title: "Dune Part Three",
                Type: TrackedTitleType.Movie,
                ReleaseDate: new DateOnly(2026, 12, 18))
        ];
    }
}
