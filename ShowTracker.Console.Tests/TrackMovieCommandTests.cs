using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class TrackMovieCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Tracks_Movie_By_Title()
    {
        string? requestedTitle = null;

        var service = new TestTrackMovieService
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
            {
                requestedTitle = title;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "12",
                        Title: "Star Wars",
                        Type: TrackedTitleType.Movie,
                        Platform: platform));
            }
        };

        var command = new TrackMovieCommand(service);

        await command.ExecuteAsync(["track-movie", "Star", "Wars"]);

        Assert.Equal("Star Wars", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Tracks_Movie_By_Provider_Id()
    {
        string? requestedTitle = null;

        var service = new TestTrackMovieService
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
            {
                requestedTitle = title;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "12",
                        Title: "Star Wars",
                        Type: TrackedTitleType.Movie,
                        Platform: platform));
            }
        };

        var command = new TrackMovieCommand(service);

        await command.ExecuteAsync(["track-movie", "12"]);

        Assert.Equal("12", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Trims_Title_Or_Provider_Id()
    {
        string? requestedTitle = null;

        var service = new TestTrackMovieService
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
            {
                requestedTitle = title;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "12",
                        Title: "Star Wars",
                        Type: TrackedTitleType.Movie,
                        Platform: platform));
            }
        };

        var command = new TrackMovieCommand(service);

        await command.ExecuteAsync(["track-movie", "  Star Wars  "]);

        Assert.Equal("Star Wars", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Tracked_Movie_Message()
    {
        var service = new TestTrackMovieService
        {
            TrackMovieAsyncHandler = (_, platform, _) =>
                Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "12",
                        Title: "Star Wars",
                        Type: TrackedTitleType.Movie,
                        Platform: platform))
        };

        var command = new TrackMovieCommand(service);

        var output = await command.ExecuteAsync(["track-movie", "Star", "Wars"]);

        Assert.Contains("Tracked movie", output);
        Assert.Contains("Star Wars", output);
        Assert.Contains("12", output);
    }

    [Fact]
    public async Task ExecuteAsync_Passes_Platform_When_Provided()
    {
        string? requestedPlatform = null;

        var service = new TestTrackMovieService
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
            {
                requestedPlatform = platform;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "12",
                        Title: title,
                        Type: TrackedTitleType.Movie,
                        Platform: platform));
            }
        };

        var command = new TrackMovieCommand(service);

        await command.ExecuteAsync(["track-movie", "Star", "Wars", "--platform", "Disney+"]);

        Assert.Equal("Disney+", requestedPlatform);
    }

    [Fact]
    public async Task ExecuteAsync_Passes_Multi_Word_Platform_When_Provided()
    {
        string? requestedPlatform = null;

        var service = new TestTrackMovieService
        {
            TrackMovieAsyncHandler = (title, platform, _) =>
            {
                requestedPlatform = platform;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "12",
                        Title: title,
                        Type: TrackedTitleType.Movie,
                        Platform: platform));
            }
        };

        var command = new TrackMovieCommand(service);

        await command.ExecuteAsync(["track-movie", "Star", "Wars", "--platform", "Amazon", "Prime"]);

        Assert.Equal("Amazon Prime", requestedPlatform);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Title()
    {
        var service = new TestTrackMovieService();

        var command = new TrackMovieCommand(service);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["track-movie"]));
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Platform_Value()
    {
        var service = new TestTrackMovieService();

        var command = new TrackMovieCommand(service);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["track-movie", "Star", "Wars", "--platform"]));
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