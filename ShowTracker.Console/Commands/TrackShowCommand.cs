using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class TrackShowCommand : IConsoleCommand
{
    private readonly ITrackShowService _trackShowService;

    public TrackShowCommand(ITrackShowService trackShowService)
    {
        _trackShowService = trackShowService ?? throw new ArgumentNullException(nameof(trackShowService));
    }

    public async Task<string> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var (title, platform) = TrackCommandArgumentParser.Parse(args, "Show title is required.");

        var trackedTitle = await _trackShowService.TrackShowAsync(
            title,
            platform,
            cancellationToken);

        return $"Tracked show: {trackedTitle.Title} [{trackedTitle.ProviderId}]";
    }
}