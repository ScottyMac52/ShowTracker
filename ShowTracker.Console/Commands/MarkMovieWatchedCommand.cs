using ShowTracker.Application.Services.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class MarkMovieWatchedCommand : IConsoleCommand
{
    private readonly IMarkMovieWatchedService _markMovieWatchedService;

    public MarkMovieWatchedCommand(IMarkMovieWatchedService markMovieWatchedService)
    {
        _markMovieWatchedService = markMovieWatchedService ?? throw new ArgumentNullException(nameof(markMovieWatchedService));
    }

    public async Task<string> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var title = CommandArgumentParser.RequireTextAfterCommand(args, "Movie title is required.");

        await _markMovieWatchedService.MarkMovieWatchedAsync(title, cancellationToken);

        return $"Marked watched: {title}";
    }
}