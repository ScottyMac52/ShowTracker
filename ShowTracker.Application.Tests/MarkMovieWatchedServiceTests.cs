using ShowTracker.Application.Services;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class MarkMovieWatchedServiceTests
{
    [Fact]
    public async Task MarkMovieWatchedAsync_Saves_Progress()
    {
        WatchProgress? savedProgress = null;

        var repository = new TestWatchProgressRepository
        {
            SaveAsyncHandler = (progress, _) =>
            {
                savedProgress = progress;
                return Task.CompletedTask;
            }
        };

        var service = new MarkMovieWatchedService(repository);

        await service.MarkMovieWatchedAsync("Dune: Part Two");

        Assert.NotNull(savedProgress);
        Assert.Equal("Dune: Part Two", savedProgress!.ShowTitle);
        Assert.Null(savedProgress.LastWatchedSeason);
        Assert.Null(savedProgress.LastWatchedEpisode);
    }

    [Fact]
    public async Task MarkMovieWatchedAsync_Trims_Movie_Title()
    {
        WatchProgress? savedProgress = null;

        var repository = new TestWatchProgressRepository
        {
            SaveAsyncHandler = (progress, _) =>
            {
                savedProgress = progress;
                return Task.CompletedTask;
            }
        };

        var service = new MarkMovieWatchedService(repository);

        await service.MarkMovieWatchedAsync("  Dune: Part Two  ");

        Assert.NotNull(savedProgress);
        Assert.Equal("Dune: Part Two", savedProgress!.ShowTitle);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MarkMovieWatchedAsync_Rejects_Blank_Title(string movieTitle)
    {
        var repository = new TestWatchProgressRepository();
        var service = new MarkMovieWatchedService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.MarkMovieWatchedAsync(movieTitle));
    }
}