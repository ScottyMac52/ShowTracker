using System.Text;
using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands.Interfaces;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Commands;

public sealed class SearchCommand : IConsoleCommand
{
    private readonly ISearchTitlesService _searchTitlesService;

    public SearchCommand(ISearchTitlesService searchTitlesService)
    {
        _searchTitlesService = searchTitlesService
            ?? throw new ArgumentNullException(nameof(searchTitlesService));
    }

    public async Task<string> ExecuteAsync(
        string[] args,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length < 2)
            throw new ArgumentException("Search query is required.", nameof(args));

        var query = string.Join(
            ' ',
            args.Skip(1))
            .Trim();

        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Search query is required.", nameof(args));

        var results = await _searchTitlesService.SearchTitlesAsync(
            query,
            cancellationToken);

        if (results.Count == 0)
            return "No titles found.";

        return SearchCommandFormatter.Format(results);
    }
}