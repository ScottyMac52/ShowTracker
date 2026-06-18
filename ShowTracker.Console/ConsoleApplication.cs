using ShowTracker.Console.Commands;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console;

public sealed class ConsoleApplication : IConsoleApplication
{
    private readonly CommandRouter _commandRouter;

    public ConsoleApplication(CommandRouter commandRouter)
    {
        _commandRouter = commandRouter
            ?? throw new ArgumentNullException(nameof(commandRouter));
    }

    public async Task<int> RunAsync(
        string[] args,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(args);

        try
        {
            var output = await _commandRouter.ExecuteAsync(
                args,
                cancellationToken);

            System.Console.WriteLine(output);

            return 0;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            System.Console.Error.WriteLine(ex.Message);

            return 1;
        }
    }
}