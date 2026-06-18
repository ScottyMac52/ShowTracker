using ShowTracker.Application;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class GetNextReleaseServiceTests
{
    [Fact]
    public async Task GetNextReleaseAsync_Returns_Next_Release_For_Tracked_Title()
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
            GetNextReleaseAsyncHandler = (showTitle, _) =>
                Task.FromResult<UpcomingRelease?>(
                    new UpcomingRelease(
                        ProviderId: "trakt:show:12345",
                        Title: showTitle,
                        Type: TrackedTitleType.Show,
                        ReleaseDate: new DateOnly(2026, 7, 10),
                        SeasonNumber: 2,
                        EpisodeNumber: 1,
                        EpisodeTitle: "Episode One",
                        Platform: "Disney Plus"))
        };

        var service = new GetNextReleaseService(provider, repository);

        var release = await service.GetNextReleaseAsync("Andor");

        Assert.NotNull(release);
        Assert.Equal("Andor", release!.Title);
        Assert.Equal("trakt:show:12345", release.ProviderId);
        Assert.Equal(2, release.SeasonNumber);
        Assert.Equal(1, release.EpisodeNumber);
    }

    [Fact]
    public async Task GetNextReleaseAsync_Returns_Null_When_Title_Is_Not_Tracked()
    {
        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>([])
        };

        var providerCalled = false;

        var provider = new TestTitleTrackingProvider
        {
            GetNextReleaseAsyncHandler = (_, _) =>
            {
                providerCalled = true;
                return Task.FromResult<UpcomingRelease?>(null);
            }
        };

        var service = new GetNextReleaseService(provider, repository);

        var release = await service.GetNextReleaseAsync("Andor");

        Assert.Null(release);
        Assert.False(providerCalled);
    }

    [Fact]
    public async Task GetNextReleaseAsync_Returns_Null_When_Provider_Has_No_Release()
    {
        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new(
                        ProviderId: "trakt:show:12345",
                        Title: "Andor",
                        Type: TrackedTitleType.Show)
                ])
        };

        var provider = new TestTitleTrackingProvider
        {
            GetNextReleaseAsyncHandler = (_, _) =>
                Task.FromResult<UpcomingRelease?>(null)
        };

        var service = new GetNextReleaseService(provider, repository);

        var release = await service.GetNextReleaseAsync("Andor");

        Assert.Null(release);
    }

    [Fact]
    public async Task GetNextReleaseAsync_Trims_Title()
    {
        string? requestedTitle = null;

        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new(
                        ProviderId: "trakt:show:12345",
                        Title: "Andor",
                        Type: TrackedTitleType.Show)
                ])
        };

        var provider = new TestTitleTrackingProvider
        {
            GetNextReleaseAsyncHandler = (showTitle, _) =>
            {
                requestedTitle = showTitle;
                return Task.FromResult<UpcomingRelease?>(null);
            }
        };

        var service = new GetNextReleaseService(provider, repository);

        await service.GetNextReleaseAsync("  Andor  ");

        Assert.Equal("Andor", requestedTitle);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetNextReleaseAsync_Rejects_Blank_Title(string showTitle)
    {
        var provider = new TestTitleTrackingProvider();
        var repository = new TestTrackedTitleRepository();

        var service = new GetNextReleaseService(provider, repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetNextReleaseAsync(showTitle));
    }
}