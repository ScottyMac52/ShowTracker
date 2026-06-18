using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class TrackMovieCommand : IConsoleCommand
{
    private readonly ITrackMovieService _trackMovieService;

    public TrackMovieCommand(ITrackMovieService trackMovieService)
    {
        _trackMovieService = trackMovieService ?? throw new ArgumentNullException(nameof(trackMovieService));
    }

    public async Task<string> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var title = CommandArgumentParser.RequireTextAfterCommand(args, "Movie title is required.");

        var trackedTitle = await _trackMovieService.TrackMovieAsync(title, cancellationToken: cancellationToken);

        return $"Tracked movie: {trackedTitle.Title} [{trackedTitle.ProviderId}]";
    }
}