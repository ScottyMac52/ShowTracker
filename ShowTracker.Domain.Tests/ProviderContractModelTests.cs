using ShowTracker.Domain.Models;

namespace ShowTracker.Domain.Tests;

public sealed class ProviderContractModelTests
{
    [Fact]
    public void TrackedTitle_Can_Represent_Show()
    {
        var title = new TrackedTitle(
            ProviderId: "trakt:show:your-friends-and-neighbors",
            Title: "Your Friends & Neighbors",
            Type: TrackedTitleType.Show,
            Platform: "Apple TV");

        Assert.Equal("trakt:show:your-friends-and-neighbors", title.ProviderId);
        Assert.Equal("Your Friends & Neighbors", title.Title);
        Assert.Equal(TrackedTitleType.Show, title.Type);
        Assert.Equal("Apple TV", title.Platform);
    }

    [Fact]
    public void UpcomingRelease_Can_Represent_Show_Episode()
    {
        var release = new UpcomingRelease(
            ProviderId: "trakt:episode:123",
            Title: "Your Friends & Neighbors",
            Type: TrackedTitleType.Show,
            ReleaseDate: new DateOnly(2026, 7, 10),
            SeasonNumber: 2,
            EpisodeNumber: 1,
            EpisodeTitle: "Episode One",
            Platform: "Apple TV");

        Assert.Equal(TrackedTitleType.Show, release.Type);
        Assert.Equal(new DateOnly(2026, 7, 10), release.ReleaseDate);
        Assert.Equal(2, release.SeasonNumber);
        Assert.Equal(1, release.EpisodeNumber);
    }

    [Fact]
    public void WatchProgress_Can_Represent_Last_And_Next_Episode()
    {
        var progress = new WatchProgress(
            ProviderId: "trakt:show:andor",
            ShowTitle: "Andor",
            LastWatchedSeason: 2,
            LastWatchedEpisode: 5,
            LastWatchedEpisodeTitle: "Last Episode",
            NextSeason: 2,
            NextEpisode: 6,
            NextEpisodeTitle: "Next Episode");

        Assert.Equal("Andor", progress.ShowTitle);
        Assert.Equal(2, progress.LastWatchedSeason);
        Assert.Equal(5, progress.LastWatchedEpisode);
        Assert.Equal(2, progress.NextSeason);
        Assert.Equal(6, progress.NextEpisode);
    }
}