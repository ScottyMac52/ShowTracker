using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class MarkMovieWatchedServiceTests
{
    [Fact]
    public async Task MarkMovieWatchedAsync_Calls_Provider()
    {
        string? capturedMovieTitle = null;

        var provider = new TestWatchTrackingProvider
        {
            MarkMovieWatchedAsyncHandler = (movieTitle, _) =>
            {
                capturedMovieTitle = movieTitle;
                return Task.CompletedTask;
            }
        };

        var service = new MarkMovieWatchedService(provider);

        await service.MarkMovieWatchedAsync("Dune Part Two");

        Assert.Equal("Dune Part Two", capturedMovieTitle);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MarkMovieWatchedAsync_Rejects_Blank_Title(string movieTitle)
    {
        var provider = new TestWatchTrackingProvider();
        var service = new MarkMovieWatchedService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.MarkMovieWatchedAsync(movieTitle));
    }

    [Fact]
    public async Task MarkMovieWatchedAsync_Trims_Title()
    {
        string? capturedMovieTitle = null;

        var provider = new TestWatchTrackingProvider
        {
            MarkMovieWatchedAsyncHandler = (movieTitle, _) =>
            {
                capturedMovieTitle = movieTitle;
                return Task.CompletedTask;
            }
        };

        var service = new MarkMovieWatchedService(provider);

        await service.MarkMovieWatchedAsync("  Dune Part Two  ");

        Assert.Equal("Dune Part Two", capturedMovieTitle);
    }
}