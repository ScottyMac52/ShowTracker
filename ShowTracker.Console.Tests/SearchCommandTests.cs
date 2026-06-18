using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class SearchCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Searches_For_Query()
    {
        string? searchedQuery = null;

        var service = new TestSearchTitlesService
        {
            SearchTitlesAsyncHandler = (query, _) =>
            {
                searchedQuery = query;
                return Task.FromResult<IReadOnlyList<TitleSearchResult>>([]);
            }
        };

        var command = new SearchCommand(service);

        await command.ExecuteAsync(["search", "andor"]);

        Assert.Equal("andor", searchedQuery);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Formatted_Search_Results()
    {
        var service = new TestSearchTitlesService
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                        ProviderId: "trakt:show:12345",
                        Title: "Andor",
                        Type: TrackedTitleType.Show,
                        Year: 2022),

                    new(
                        ProviderId: "trakt:movie:654321",
                        Title: "Dune: Part Two",
                        Type: TrackedTitleType.Movie,
                        Year: 2024)
                ])
        };

        var command = new SearchCommand(service);

        var output = await command.ExecuteAsync(["search", "andor"]);

        Assert.Contains("Andor", output);
        Assert.Contains("Show", output);
        Assert.Contains("2022", output);
        Assert.Contains("Dune: Part Two", output);
        Assert.Contains("Movie", output);
        Assert.Contains("2024", output);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Message_When_No_Results_Are_Found()
    {
        var service = new TestSearchTitlesService
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>([])
        };

        var command = new SearchCommand(service);

        var output = await command.ExecuteAsync(["search", "missing"]);

        Assert.Equal("No titles found.", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Query()
    {
        var service = new TestSearchTitlesService();
        var command = new SearchCommand(service);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["search"]));
    }

    private sealed class TestSearchTitlesService : ISearchTitlesService
    {
        public Func<string, CancellationToken, Task<IReadOnlyList<TitleSearchResult>>>? SearchTitlesAsyncHandler { get; set; }

        public Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            if (SearchTitlesAsyncHandler is null)
                throw new NotImplementedException();

            return SearchTitlesAsyncHandler(query, cancellationToken);
        }
    }
}