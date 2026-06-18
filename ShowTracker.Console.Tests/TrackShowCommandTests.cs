using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class TrackShowCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Tracks_Show()
    {
        string? requestedTitle = null;

        var service = new TestTrackShowService
        {
            TrackShowAsyncHandler = (title, platform, _) =>
            {
                requestedTitle = title;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "trakt:show:12345",
                        Title: title,
                        Type: TrackedTitleType.Show));
            }
        };

        var command = new TrackShowCommand(service);

        await command.ExecuteAsync(["track-show", "Andor"]);

        Assert.Equal("Andor", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Trims_Title()
    {
        string? requestedTitle = null;

        var service = new TestTrackShowService
        {
            TrackShowAsyncHandler = (title, platform, _) =>
            {
                requestedTitle = title;

                return Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "trakt:show:12345",
                        Title: title,
                        Type: TrackedTitleType.Show));
            }
        };

        var command = new TrackShowCommand(service);

        await command.ExecuteAsync(["track-show", "  Andor  "]);

        Assert.Equal("Andor", requestedTitle);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Tracked_Show_Message()
    {
        var service = new TestTrackShowService
        {
            TrackShowAsyncHandler = (title, platform, _) =>
                Task.FromResult(
                    new TrackedTitle(
                        ProviderId: "trakt:show:12345",
                        Title: "Andor",
                        Type: TrackedTitleType.Show))
        };

        var command = new TrackShowCommand(service);

        var output = await command.ExecuteAsync(
            ["track-show", "Andor"]);

        Assert.Contains("Tracked show", output);
        Assert.Contains("Andor", output);
        Assert.Contains("trakt:show:12345", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Title()
    {
        var service = new TestTrackShowService();

        var command = new TrackShowCommand(service);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["track-show"]));
    }
}