using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class SearchMovieCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Searches_For_Movie_Query()
    {
        string? searchedQuery = null;

        var service = new TestSearchTitlesService
        {
            SearchMoviesAsyncHandler = (query, _) =>
            {
                searchedQuery = query;
                return Task.FromResult<IReadOnlyList<TitleSearchResult>>([]);
            }
        };

        var command = new SearchMovieCommand(service);

        await command.ExecuteAsync(["search-movie", "star", "wars"]);

        Assert.Equal("star wars", searchedQuery);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Formatted_Movie_Results()
    {
        var service = new TestSearchTitlesService
        {
            SearchMoviesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                        ProviderId: "12",
                        Title: "Star Wars",
                        Type: TrackedTitleType.Movie,
                        Year: 1977)
                ])
        };

        var command = new SearchMovieCommand(service);

        var output = await command.ExecuteAsync(["search-movie", "Star", "Wars"]);

        Assert.Contains("Star Wars", output);
        Assert.Contains("Movie", output);
        Assert.Contains("1977", output);
        Assert.Contains("12", output);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Message_When_No_Movie_Results_Are_Found()
    {
        var service = new TestSearchTitlesService
        {
            SearchMoviesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>([])
        };

        var command = new SearchMovieCommand(service);

        var output = await command.ExecuteAsync(["search-movie", "missing"]);

        Assert.Equal("No titles found.", output);
    }

    [Fact]
    public async Task ExecuteAsync_Rejects_Missing_Query()
    {
        var command = new SearchMovieCommand(new TestSearchTitlesService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            command.ExecuteAsync(["search-movie"]));
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