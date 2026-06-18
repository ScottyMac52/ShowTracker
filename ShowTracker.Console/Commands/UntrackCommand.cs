using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class UntrackCommand : IConsoleCommand
{
    private readonly IUntrackTitleService _untrackTitleService;

    public UntrackCommand(IUntrackTitleService untrackTitleService)
    {
        _untrackTitleService = untrackTitleService ?? throw new ArgumentNullException(nameof(untrackTitleService));
    }

    public async Task<string> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var providerId = CommandArgumentParser.RequireTextAfterCommand(args, "Provider id is required.");

        await _untrackTitleService.UntrackAsync(providerId, cancellationToken);

        return $"Untracked title: {providerId}";
    }
}