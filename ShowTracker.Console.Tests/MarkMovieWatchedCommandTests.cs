using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;

namespace ShowTracker.Console.Tests;

public sealed class MarkMovieWatchedCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Marks_Movie_Watched()
    {
        string? requestedTitle = null;

        var service = new TestMarkMovieWatchedService
        {
            MarkMovieWatchedAsyncHandler = (title, _) =>
            {
                requestedTitle = title;
                return Task.CompletedTask;
            }
        };

        var command = new MarkMovieWatchedCommand(service);

        await command.ExecuteAsync(["watched-movie", "Dune", "Part", "Two"]);

        Assert.Equal("Dune Part Two", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Watched_Message()
    {
        var service = new TestMarkMovieWatchedService
        {
            MarkMovieWatchedAsyncHandler = (_, _) => Task.CompletedTask
        };

        var command = new MarkMovieWatchedCommand(service);

        var output = await command.ExecuteAsync(["watched-movie", "Dune", "Part", "Two"]);

        Assert.Contains("Marked watched", output);
        Assert.Contains("Dune Part Two", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Title()
    {
        var command = new MarkMovieWatchedCommand(new TestMarkMovieWatchedService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["watched-movie"]));
    }

    private sealed class TestMarkMovieWatchedService : IMarkMovieWatchedService
    {
        public Func<string, CancellationToken, Task>? MarkMovieWatchedAsyncHandler { get; set; }

        public Task MarkMovieWatchedAsync(
            string movieTitle,
            CancellationToken cancellationToken = default)
        {
            if (MarkMovieWatchedAsyncHandler is null)
                throw new NotImplementedException();

            return MarkMovieWatchedAsyncHandler(movieTitle, cancellationToken);
        }
    }
}