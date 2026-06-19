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
        var titleOrProviderId = CommandArgumentParser.RequireTextAfterCommand(args, "Title or provider id is required.");

        await _untrackTitleService.UntrackAsync(titleOrProviderId, cancellationToken);

        return $"Untracked title: {titleOrProviderId}";
    }
}