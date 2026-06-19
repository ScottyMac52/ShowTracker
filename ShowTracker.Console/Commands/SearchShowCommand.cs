using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class SearchShowCommand : IConsoleCommand
{
    private readonly ISearchTitlesService _searchTitlesService;

    public SearchShowCommand(ISearchTitlesService searchTitlesService)
    {
        _searchTitlesService = searchTitlesService;
    }

    public async Task<string> ExecuteAsync(
        string[] args,
        CancellationToken cancellationToken = default)
    {
        var query = CommandArgumentParser.RequireTextAfterCommand(
            args,
            "Show search query is required.");

        var results = await _searchTitlesService.SearchShowsAsync(
            query,
            cancellationToken);

        return SearchCommandFormatter.Format(results);
    }
}