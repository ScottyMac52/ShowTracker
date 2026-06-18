using ShowTracker.Application.Services.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class MarkEpisodeWatchedCommand : IConsoleCommand
{
    private readonly IMarkEpisodeWatchedService _markEpisodeWatchedService;

    public MarkEpisodeWatchedCommand(IMarkEpisodeWatchedService markEpisodeWatchedService)
    {
        _markEpisodeWatchedService = markEpisodeWatchedService ?? throw new ArgumentNullException(nameof(markEpisodeWatchedService));
    }

    public async Task<string> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var (title, season, episode) = CommandArgumentParser.RequireEpisodeArguments(args);

        await _markEpisodeWatchedService.MarkEpisodeWatchedAsync(title, season, episode, cancellationToken);

        return $"Marked watched: {title} S{season}E{episode}";
    }
}