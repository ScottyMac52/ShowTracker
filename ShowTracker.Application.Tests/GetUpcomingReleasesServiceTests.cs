using ShowTracker.Application;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class GetUpcomingReleasesServiceTests
{
    [Fact]
    public async Task GetUpcomingReleasesAsync_Returns_Releases_For_Tracked_Titles()
    {
        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new(
                        ProviderId: "trakt:show:12345",
                        Title: "Andor",
                        Type: TrackedTitleType.Show,
                        Platform: "Disney Plus")
                ])
        };

        var provider = new TestTitleTrackingProvider
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
                        EpisodeTitle: "Episode One",
                        Platform: "Disney Plus"),

                    new(
                        ProviderId: "trakt:show:99999",
                        Title: "Unknown Show",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 7, 12),
                        SeasonNumber: 1,
                        EpisodeNumber: 1)
                ])
        };

        var service = new GetUpcomingReleasesService(provider, repository);

        var results = await service.GetUpcomingReleasesAsync();

        var result = Assert.Single(results);
        Assert.Equal("Andor", result.Title);
        Assert.Equal("trakt:show:12345", result.ProviderId);
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Returns_Empty_List_When_No_Titles_Are_Tracked()
    {
        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>([])
        };

        var providerCalled = false;

        var provider = new TestTitleTrackingProvider
        {
            GetUpcomingReleasesAsyncHandler = _ =>
            {
                providerCalled = true;
                return Task.FromResult<IReadOnlyList<UpcomingRelease>>([]);
            }
        };

        var service = new GetUpcomingReleasesService(provider, repository);

        var results = await service.GetUpcomingReleasesAsync();

        Assert.Empty(results);
        Assert.False(providerCalled);
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Returns_Empty_List_When_Provider_Has_No_Matching_Releases()
    {
        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new(
                        ProviderId: "trakt:show:12345",
                        Title: "Andor",
                        Type: TrackedTitleType.Show,
                        Platform: "Disney Plus")
                ])
        };

        var provider = new TestTitleTrackingProvider
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>(
                [
                    new(
                        ProviderId: "trakt:show:99999",
                        Title: "Unknown Show",
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 7, 12),
                        SeasonNumber: 1,
                        EpisodeNumber: 1)
                ])
        };

        var service = new GetUpcomingReleasesService(provider, repository);

        var results = await service.GetUpcomingReleasesAsync();

        Assert.Empty(results);
    }
}