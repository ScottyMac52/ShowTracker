using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class GetNextReleaseCommand : IConsoleCommand
{
    private readonly IGetNextReleaseService _getNextReleaseService;

    public GetNextReleaseCommand(
        IGetNextReleaseService getNextReleaseService)
    {
        _getNextReleaseService = getNextReleaseService
            ?? throw new ArgumentNullException(nameof(getNextReleaseService));
    }

    public async Task<string> ExecuteAsync(
        string[] args,
        CancellationToken cancellationToken = default)
    {
        var title = CommandArgumentParser.RequireTextAfterCommand(
            args,
            "Title is required.");

        var release = await _getNextReleaseService.GetNextReleaseAsync(
            title,
            cancellationToken);

        if (release is null)
            return $"No upcoming release found for: {title}";

        return UpcomingReleaseFormatter.Format(release);
    }
}