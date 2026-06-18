using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class CommandRouter
{
    private readonly IReadOnlyDictionary<string, IConsoleCommand> _commands;

    public CommandRouter(
        IReadOnlyDictionary<string, IConsoleCommand> commands)
    {
        _commands = commands
            ?? throw new ArgumentNullException(nameof(commands));
    }

    public Task<string> ExecuteAsync(
        string[] args,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            throw new ArgumentException("Command is required.", nameof(args));

        var commandName = args[0].Trim();

        if (!_commands.TryGetValue(commandName, out var command))
            throw new ArgumentException($"Unknown command: {commandName}", nameof(args));

        return command.ExecuteAsync(args, cancellationToken);
    }
}