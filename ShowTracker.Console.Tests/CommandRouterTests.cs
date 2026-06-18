using ShowTracker.Console.Commands;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Tests;

public sealed class CommandRouterTests
{
    [Fact]
    public async Task ExecuteAsync_Routes_Search_Command()
    {
        var command = new TestConsoleCommand
        {
            ExecuteAsyncHandler = (_, _) =>
                Task.FromResult("search-result")
        };

        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase)
            {
                ["search"] = command
            });

        var output = await router.ExecuteAsync(["search", "andor"]);

        Assert.Equal("search-result", output);
        Assert.Equal(["search", "andor"], command.ReceivedArgs);
    }

    [Fact]
    public async Task ExecuteAsync_Routes_Track_Show_Command()
    {
        var command = new TestConsoleCommand
        {
            ExecuteAsyncHandler = (_, _) =>
                Task.FromResult("track-show-result")
        };

        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase)
            {
                ["track-show"] = command
            });

        var output = await router.ExecuteAsync(["track-show", "Andor"]);

        Assert.Equal("track-show-result", output);
        Assert.Equal(["track-show", "Andor"], command.ReceivedArgs);
    }

    [Fact]
    public async Task ExecuteAsync_Routes_Commands_Case_Insensitively()
    {
        var command = new TestConsoleCommand
        {
            ExecuteAsyncHandler = (_, _) =>
                Task.FromResult("search-result")
        };

        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase)
            {
                ["search"] = command
            });

        var output = await router.ExecuteAsync(["SEARCH", "andor"]);

        Assert.Equal("search-result", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Unknown_Command()
    {
        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            router.ExecuteAsync(["missing"]));
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Command()
    {
        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            router.ExecuteAsync([]));
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Blank_Command()
    {
        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            router.ExecuteAsync([" "]));
    }

    private sealed class TestConsoleCommand : IConsoleCommand
    {
        public string[]? ReceivedArgs { get; private set; }

        public Func<string[], CancellationToken, Task<string>>? ExecuteAsyncHandler { get; set; }

        public Task<string> ExecuteAsync(
            string[] args,
            CancellationToken cancellationToken = default)
        {
            ReceivedArgs = args;

            if (ExecuteAsyncHandler is null)
                throw new NotImplementedException();

            return ExecuteAsyncHandler(args, cancellationToken);
        }
    }
}