using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class SearchShowCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Searches_For_Show_Query()
    {
        string? searchedQuery = null;

        var service = new TestSearchTitlesService
        {
            SearchShowsAsyncHandler = (query, _) =>
            {
                searchedQuery = query;
                return Task.FromResult<IReadOnlyList<TitleSearchResult>>([]);
            }
        };

        var command = new SearchShowCommand(service);

        await command.ExecuteAsync(["search-show", "andor"]);

        Assert.Equal("andor", searchedQuery);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Formatted_Show_Results()
    {
        var service = new TestSearchTitlesService
        {
            SearchShowsAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                        ProviderId: "188205",
                        Title: "FROM",
                        Type: TrackedTitleType.Show,
                        Year: 2022)
                ])
        };

        var command = new SearchShowCommand(service);

        var output = await command.ExecuteAsync(["search-show", "FROM"]);

        Assert.Contains("FROM", output);
        Assert.Contains("Show", output);
        Assert.Contains("2022", output);
        Assert.Contains("188205", output);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Message_When_No_Show_Results_Are_Found()
    {
        var service = new TestSearchTitlesService
        {
            SearchShowsAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>([])
        };

        var command = new SearchShowCommand(service);

        var output = await command.ExecuteAsync(["search-show", "missing"]);

        Assert.Equal("No titles found.", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Query()
    {
        var command = new SearchShowCommand(new TestSearchTitlesService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["search-show"]));
    }

    private sealed class TestSearchTitlesService : ISearchTitlesService
    {
        public Func<string, CancellationToken, Task<IReadOnlyList<TitleSearchResult>>>? SearchTitlesAsyncHandler { get; set; }

        public Func<string, CancellationToken, Task<IReadOnlyList<TitleSearchResult>>>? SearchShowsAsyncHandler { get; set; }

        public Func<string, CancellationToken, Task<IReadOnlyList<TitleSearchResult>>>? SearchMoviesAsyncHandler { get; set; }

        public Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            if (SearchTitlesAsyncHandler is null)
                throw new NotImplementedException();

            return SearchTitlesAsyncHandler(query, cancellationToken);
        }

        public Task<IReadOnlyList<TitleSearchResult>> SearchShowsAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            if (SearchShowsAsyncHandler is null)
                throw new NotImplementedException();

            return SearchShowsAsyncHandler(query, cancellationToken);
        }

        public Task<IReadOnlyList<TitleSearchResult>> SearchMoviesAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            if (SearchMoviesAsyncHandler is null)
                throw new NotImplementedException();

            return SearchMoviesAsyncHandler(query, cancellationToken);
        }
    }
}