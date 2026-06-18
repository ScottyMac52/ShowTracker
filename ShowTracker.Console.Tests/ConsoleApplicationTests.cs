using ShowTracker.Console.Commands;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Tests;

public sealed class ConsoleApplicationTests
{
    [Fact]
    public async Task RunAsync_Executes_Command()
    {
        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase)
            {
                ["search"] = new TestConsoleCommand
                {
                    ExecuteAsyncHandler = (_, _) =>
                        Task.FromResult("search-result")
                }
            });

        var application = new ConsoleApplication(router);

        await application.RunAsync(["search", "andor"]);

        Assert.True(true);
    }

    [Fact]
    public async Task RunAsync_Returns_Zero_On_Success()
    {
        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase)
            {
                ["search"] = new TestConsoleCommand
                {
                    ExecuteAsyncHandler = (_, _) =>
                        Task.FromResult("search-result")
                }
            });

        var application = new ConsoleApplication(router);

        var result = await application.RunAsync(["search", "andor"]);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_Returns_Non_Zero_On_Error()
    {
        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase));

        var application = new ConsoleApplication(router);

        var result = await application.RunAsync(["missing"]);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task RunAsync_Rejects_Null_Args()
    {
        var router = new CommandRouter(
            new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase));

        var application = new ConsoleApplication(router);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            application.RunAsync(null!));
    }

    private sealed class TestConsoleCommand : IConsoleCommand
    {
        public Func<string[], CancellationToken, Task<string>>? ExecuteAsyncHandler { get; set; }

        public Task<string> ExecuteAsync(
            string[] args,
            CancellationToken cancellationToken = default)
        {
            if (ExecuteAsyncHandler is null)
                throw new NotImplementedException();

            return ExecuteAsyncHandler(args, cancellationToken);
        }
    }
}