using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class SearchMovieCommand : IConsoleCommand
{
    private readonly ISearchTitlesService _searchTitlesService;

    public SearchMovieCommand(ISearchTitlesService searchTitlesService)
    {
        _searchTitlesService = searchTitlesService;
    }

    public async Task<string> ExecuteAsync(
        string[] args,
        CancellationToken cancellationToken = default)
    {
        var query = CommandArgumentParser.RequireTextAfterCommand(
            args,
            "Movie search query is required.");

        var results = await _searchTitlesService.SearchMoviesAsync(
            query,
            cancellationToken);

        return SearchCommandFormatter.Format(results);
    }
}