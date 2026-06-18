using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class GetNextEpisodeCommand : IConsoleCommand
{
    private readonly IGetNextEpisodeToWatchService _getNextEpisodeToWatchService;

    public GetNextEpisodeCommand(IGetNextEpisodeToWatchService getNextEpisodeToWatchService)
    {
        _getNextEpisodeToWatchService = getNextEpisodeToWatchService ?? throw new ArgumentNullException(nameof(getNextEpisodeToWatchService));
    }

    public async Task<string> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var title = CommandArgumentParser.RequireTextAfterCommand(args, "Show title is required.");

        var episode = await _getNextEpisodeToWatchService.GetNextEpisodeToWatchAsync(title, cancellationToken);

        if (episode is null)
            return "No next episode found.";

        return $"Next episode: {episode.ShowTitle} S{episode.SeasonNumber}E{episode.EpisodeNumber} {episode.EpisodeTitle}";
    }
}