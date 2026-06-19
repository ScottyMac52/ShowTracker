using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;

namespace ShowTracker.Console.Tests;

public sealed class UntrackCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Untracks_Title()
    {
        string? removedTitleOrProviderId = null;

        var service = new TestUntrackTitleService
        {
            UntrackAsyncHandler = (titleOrProviderId, _) =>
            {
                removedTitleOrProviderId = titleOrProviderId;
                return Task.CompletedTask;
            }
        };

        var command = new UntrackCommand(service);

        await command.ExecuteAsync(["untrack", "The Boys"]);

        Assert.Equal("The Boys", removedTitleOrProviderId);
    }

    [Fact]
    public async Task ExecuteAsync_Untracks_Provider_Id()
    {
        string? removedTitleOrProviderId = null;

        var service = new TestUntrackTitleService
        {
            UntrackAsyncHandler = (titleOrProviderId, _) =>
            {
                removedTitleOrProviderId = titleOrProviderId;
                return Task.CompletedTask;
            }
        };

        var command = new UntrackCommand(service);

        await command.ExecuteAsync(["untrack", "139960"]);

        Assert.Equal("139960", removedTitleOrProviderId);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Untracked_Message()
    {
        var service = new TestUntrackTitleService
        {
            UntrackAsyncHandler = (_, _) => Task.CompletedTask
        };

        var command = new UntrackCommand(service);

        var output = await command.ExecuteAsync(["untrack", "The Boys"]);

        Assert.Contains("Untracked title", output);
        Assert.Contains("The Boys", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Title_Or_Provider_Id()
    {
        var command = new UntrackCommand(new TestUntrackTitleService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["untrack"]));
    }

    private sealed class TestUntrackTitleService : IUntrackTitleService
    {
        public Func<string, CancellationToken, Task>? UntrackAsyncHandler { get; set; }

        public Task UntrackAsync(
            string titleOrProviderId,
            CancellationToken cancellationToken = default)
        {
            if (UntrackAsyncHandler is null)
                throw new NotImplementedException();

            return UntrackAsyncHandler(titleOrProviderId, cancellationToken);
        }
    }
}