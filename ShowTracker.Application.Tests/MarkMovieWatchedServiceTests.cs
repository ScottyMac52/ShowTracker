using ShowTracker.Application;
using ShowTracker.Domain;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Tests;

public sealed class MarkMovieWatchedServiceTests
{
    [Fact]
    public async Task MarkMovieWatchedAsync_Calls_Provider()
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new MarkMovieWatchedService(provider);

        await service.MarkMovieWatchedAsync("Dune Part Two");

        Assert.Equal("Dune Part Two", provider.MovieTitle);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MarkMovieWatchedAsync_Rejects_Blank_Title(
        string movieTitle)
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new MarkMovieWatchedService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.MarkMovieWatchedAsync(movieTitle));
    }

    [Fact]
    public async Task MarkMovieWatchedAsync_Trims_Title()
    {
        var provider = new FakeWatchTrackingProvider();
        var service = new MarkMovieWatchedService(provider);

        await service.MarkMovieWatchedAsync("  Dune Part Two  ");

        Assert.Equal("Dune Part Two", provider.MovieTitle);
    }

    private sealed class FakeWatchTrackingProvider : IWatchTrackingProvider
    {
        public string? MovieTitle { get; private set; }

        public Task MarkMovieWatchedAsync(
            string movieTitle,
            CancellationToken cancellationToken = default)
        {
            MovieTitle = movieTitle;
            return Task.CompletedTask;
        }

        public Task MarkEpisodeWatchedAsync(
            string showTitle,
            int seasonNumber,
            int episodeNumber,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<WatchProgress?> GetShowProgressAsync(
            string showTitle,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}