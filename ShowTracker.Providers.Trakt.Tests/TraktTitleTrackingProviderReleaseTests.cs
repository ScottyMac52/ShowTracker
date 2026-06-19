using ShowTracker.Domain.Models;

namespace ShowTracker.Providers.Trakt.Tests;

public sealed class TraktTitleTrackingProviderReleaseTests
{
    [Fact]
    public async Task GetUpcomingReleasesAsync_Forwards_To_Release_Client()
    {
        var expected = new UpcomingRelease(
            ProviderId: "139960",
            Title: "The Boys",
            Type: TrackedTitleType.Show,
            ReleaseDate: new DateOnly(2026, 6, 20),
            SeasonNumber: 5,
            EpisodeNumber: 1,
            EpisodeTitle: "Episode One");

        var releaseClient = new TestTraktReleaseClient
        {
            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>([expected])
        };

        var provider = new TraktTitleTrackingProvider(
            new TestTraktTitleSearchClient(),
            releaseClient);

        var releases = await provider.GetUpcomingReleasesAsync();

        var release = Assert.Single(releases);
        Assert.Equal(expected, release);
    }

    [Fact]
    public async Task GetNextReleaseAsync_Forwards_Trimmed_Title_To_Release_Client()
    {
        string? capturedTitle = null;

        var expected = new UpcomingRelease(
            ProviderId: "139960",
            Title: "The Boys",
            Type: TrackedTitleType.Show,
            ReleaseDate: new DateOnly(2026, 6, 20));

        var releaseClient = new TestTraktReleaseClient
        {
            GetNextReleaseAsyncHandler = (title, _) =>
            {
                capturedTitle = title;
                return Task.FromResult<UpcomingRelease?>(expected);
            }
        };

        var provider = new TraktTitleTrackingProvider(
            new TestTraktTitleSearchClient(),
            releaseClient);

        var release = await provider.GetNextReleaseAsync("  The Boys  ");

        Assert.Equal("The Boys", capturedTitle);
        Assert.Equal(expected, release);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetNextReleaseAsync_Rejects_Blank_Title(
        string title)
    {
        var provider = new TraktTitleTrackingProvider(
            new TestTraktTitleSearchClient(),
            new TestTraktReleaseClient());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            provider.GetNextReleaseAsync(title));
    }

    private sealed class TestTraktTitleSearchClient : ITraktTitleSearchClient
    {
        public Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TitleSearchResult>> SearchShowsAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TitleSearchResult>> SearchMoviesAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class TestTraktReleaseClient : ITraktReleaseClient
    {
        public Func<CancellationToken, Task<IReadOnlyList<UpcomingRelease>>>? GetUpcomingReleasesAsyncHandler { get; set; }

        public Func<string, CancellationToken, Task<UpcomingRelease?>>? GetNextReleaseAsyncHandler { get; set; }

        public Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
            CancellationToken cancellationToken = default)
        {
            if (GetUpcomingReleasesAsyncHandler is null)
                throw new NotImplementedException();

            return GetUpcomingReleasesAsyncHandler(cancellationToken);
        }

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