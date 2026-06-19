using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class TrackShowCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Tracks_Show_By_Title()
    {
        string? requestedTitle = null;

        var service = new TestTrackShowService
        {
            TrackShowAsyncHandler = (title, platform, _) =>
            {
                requestedTitle = title;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "188205",
                        Title: "FROM",
                        Type: TrackedTitleType.Show,
                        Platform: platform));
            }
        };

        var command = new TrackShowCommand(service);

        await command.ExecuteAsync(["track-show", "FROM"]);

        Assert.Equal("FROM", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Tracks_Show_By_Provider_Id()
    {
        string? requestedTitle = null;

        var service = new TestTrackShowService
        {
            TrackShowAsyncHandler = (title, platform, _) =>
            {
                requestedTitle = title;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "188205",
                        Title: "FROM",
                        Type: TrackedTitleType.Show,
                        Platform: platform));
            }
        };

        var command = new TrackShowCommand(service);

        await command.ExecuteAsync(["track-show", "188205"]);

        Assert.Equal("188205", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Trims_Title_Or_Provider_Id()
    {
        string? requestedTitle = null;

        var service = new TestTrackShowService
        {
            TrackShowAsyncHandler = (title, platform, _) =>
            {
                requestedTitle = title;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "188205",
                        Title: "FROM",
                        Type: TrackedTitleType.Show,
                        Platform: platform));
            }
        };

        var command = new TrackShowCommand(service);

        await command.ExecuteAsync(["track-show", "  FROM  "]);

        Assert.Equal("FROM", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Tracked_Show_Message()
    {
        var service = new TestTrackShowService
        {
            TrackShowAsyncHandler = (_, platform, _) =>
                Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "188205",
                        Title: "FROM",
                        Type: TrackedTitleType.Show,
                        Platform: platform))
        };

        var command = new TrackShowCommand(service);

        var output = await command.ExecuteAsync(["track-show", "FROM"]);

        Assert.Contains("Tracked show", output);
        Assert.Contains("FROM", output);
        Assert.Contains("188205", output);
    }

    [Fact]
    public async Task ExecuteAsync_Passes_Platform_When_Provided()
    {
        string? requestedPlatform = null;

        var service = new TestTrackShowService
        {
            TrackShowAsyncHandler = (title, platform, _) =>
            {
                requestedPlatform = platform;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "188205",
                        Title: title,
                        Type: TrackedTitleType.Show,
                        Platform: platform));
            }
        };

        var command = new TrackShowCommand(service);

        await command.ExecuteAsync(["track-show", "FROM", "--platform", "MGM+"]);

        Assert.Equal("MGM+", requestedPlatform);
    }

    [Fact]
    public async Task ExecuteAsync_Passes_Multi_Word_Platform_When_Provided()
    {
        string? requestedPlatform = null;

        var service = new TestTrackShowService
        {
            TrackShowAsyncHandler = (title, platform, _) =>
            {
                requestedPlatform = platform;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "188205",
                        Title: title,
                        Type: TrackedTitleType.Show,
                        Platform: platform));
            }
        };

        var command = new TrackShowCommand(service);

        await command.ExecuteAsync(["track-show", "FROM", "--platform", "Amazon", "Prime"]);

        Assert.Equal("Amazon Prime", requestedPlatform);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Title()
    {
        var service = new TestTrackShowService();

        var command = new TrackShowCommand(service);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["track-show"]));
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Platform_Value()
    {
        var service = new TestTrackShowService();

        var command = new TrackShowCommand(service);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["track-show", "FROM", "--platform"]));
    }

    private sealed class TestTrackShowService : ITrackShowService
    {
        public Func<string, string?, CancellationToken, Task<TrackedTitle>>? TrackShowAsyncHandler { get; set; }

        public Task<TrackedTitle> TrackShowAsync(
            string showTitle,
            string? platform = null,
            CancellationToken cancellationToken = default)
        {
            if (TrackShowAsyncHandler is null)
                throw new NotImplementedException();

            return TrackShowAsyncHandler(showTitle, platform, cancellationToken);
        }
    }
}