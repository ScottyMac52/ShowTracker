using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class TrackMovieCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Tracks_Movie()
    {
        string? requestedTitle = null;

        var service = new TestTrackMovieService
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
            {
                requestedTitle = title;

                return Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:movie:654321",
                    Title: title,
                    Type: TrackedTitleType.Movie));
            }
        };

        var command = new TrackMovieCommand(service);

        await command.ExecuteAsync(["track-movie", "Dune", "Part", "Two"]);

        Assert.Equal("Dune Part Two", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Tracked_Movie_Message()
    {
        var service = new TestTrackMovieService
        {
            TrackMovieAsyncHandler = (_, _, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "trakt:movie:654321",
                    Title: "Dune: Part Two",
                    Type: TrackedTitleType.Movie))
        };

        var command = new TrackMovieCommand(service);

        var output = await command.ExecuteAsync(["track-movie", "Dune", "Part", "Two"]);

        Assert.Contains("Tracked movie", output);
        Assert.Contains("Dune: Part Two", output);
        Assert.Contains("trakt:movie:654321", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Title()
    {
        var command = new TrackMovieCommand(new TestTrackMovieService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["track-movie"]));
    }

    private sealed class TestTrackMovieService : ITrackMovieService
    {
        public Func<string, string?, CancellationToken, Task<TrackedTitle>>? TrackMovieAsyncHandler { get; set; }

        public Task<TrackedTitle> TrackMovieAsync(
            string movieTitle,
            string? platform = null,
            CancellationToken cancellationToken = default)
        {
            if (TrackMovieAsyncHandler is null)
                throw new NotImplementedException();

            return TrackMovieAsyncHandler(movieTitle, platform, cancellationToken);
        }
    }
}