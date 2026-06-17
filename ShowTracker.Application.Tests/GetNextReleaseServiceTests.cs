using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class GetNextReleaseServiceTests
{
    [Fact]
    public async Task GetNextReleaseAsync_Returns_Next_Release()
    {
        var provider = new TestTitleTrackingProvider
        {
            GetNextReleaseAsyncHandler = (showTitle, _) =>
            {
                if (!string.Equals(showTitle, "Andor", StringComparison.OrdinalIgnoreCase))
                    return Task.FromResult<UpcomingRelease?>(null);

                return Task.FromResult<UpcomingRelease?>(
                    new UpcomingRelease(
                        ProviderId: "release-1",
                        Title: "Andor",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 7, 10),
                        SeasonNumber: 2,
                        EpisodeNumber: 1,
                        EpisodeTitle: "Episode One",
                        Platform: "Disney Plus"));
            }
        };

        var service = new GetNextReleaseService(provider);

        var release = await service.GetNextReleaseAsync("Andor");

        Assert.NotNull(release);
        Assert.Equal("Andor", release!.Title);
        Assert.Equal(2, release.SeasonNumber);
        Assert.Equal(1, release.EpisodeNumber);
    }

    [Fact]
    public async Task GetNextReleaseAsync_Returns_Null_When_Not_Found()
    {
        var provider = new TestTitleTrackingProvider
        {
            GetNextReleaseAsyncHandler = (showTitle, _) =>
            {
                if (!string.Equals(showTitle, "Andor", StringComparison.OrdinalIgnoreCase))
                    return Task.FromResult<UpcomingRelease?>(null);

                return Task.FromResult<UpcomingRelease?>(
                    new UpcomingRelease(
                        ProviderId: "release-1",
                        Title: "Andor",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 7, 10),
                        SeasonNumber: 2,
                        EpisodeNumber: 1,
                        EpisodeTitle: "Episode One",
                        Platform: "Disney Plus"));
            }
        };

        var service = new GetNextReleaseService(provider);

        var release = await service.GetNextReleaseAsync("Unknown Show");

        Assert.Null(release);
    }
}
