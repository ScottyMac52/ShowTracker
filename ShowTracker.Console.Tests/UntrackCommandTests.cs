using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;

namespace ShowTracker.Console.Tests;

public sealed class UntrackCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Untracks_Title()
    {
        string? removedProviderId = null;

        var service = new TestUntrackTitleService
        {
            UntrackAsyncHandler = (providerId, _) =>
            {
                removedProviderId = providerId;
                return Task.CompletedTask;
            }
        };

        var command = new UntrackCommand(service);

        await command.ExecuteAsync(["untrack", "trakt:show:12345"]);

        Assert.Equal("trakt:show:12345", removedProviderId);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Untracked_Message()
    {
        var service = new TestUntrackTitleService
        {
            UntrackAsyncHandler = (_, _) => Task.CompletedTask
        };

        var command = new UntrackCommand(service);

        var output = await command.ExecuteAsync(["untrack", "trakt:show:12345"]);

        Assert.Contains("Untracked title", output);
        Assert.Contains("trakt:show:12345", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Provider_Id()
    {
        var command = new UntrackCommand(new TestUntrackTitleService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["untrack"]));
    }

    private sealed class TestUntrackTitleService : IUntrackTitleService
    {
        public Func<string, CancellationToken, Task>? UntrackAsyncHandler { get; set; }

        public Task UntrackAsync(
            string providerId,
            CancellationToken cancellationToken = default)
        {
            if (UntrackAsyncHandler is null)
                throw new NotImplementedException();

            return UntrackAsyncHandler(providerId, cancellationToken);
        }
    }
}