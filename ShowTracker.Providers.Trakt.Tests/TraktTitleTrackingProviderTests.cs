using ShowTracker.Domain.Models;

namespace ShowTracker.Providers.Trakt.Tests;

public sealed class TraktTitleTrackingProviderTests
{
    [Fact]
    public async Task SearchTitlesAsync_Forwards_Query_To_Search_Client()
    {
        string? capturedQuery = null;

        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (query, _) =>
            {
                capturedQuery = query;
                return Task.FromResult<IReadOnlyList<TitleSearchResult>>([]);
            }
        };

        var provider = new TraktTitleTrackingProvider(client);

        await provider.SearchTitlesAsync("Andor");

        Assert.Equal("Andor", capturedQuery);
    }

    [Fact]
    public async Task SearchTitlesAsync_Returns_Search_Client_Results()
    {
        var expected = new TitleSearchResult(
            ProviderId: "12345",
            Title: "Andor",
            Type: TrackedTitleType.Show,
            Year: 2022);

        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>([expected])
        };

        var provider = new TraktTitleTrackingProvider(client);

        var results = await provider.SearchTitlesAsync("Andor");

        var result = Assert.Single(results);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task SearchTitlesAsync_Returns_Empty_List_When_Client_Returns_Empty_List()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>([])
        };

        var provider = new TraktTitleTrackingProvider(client);

        var results = await provider.SearchTitlesAsync("Unknown");

        Assert.Empty(results);
    }

    [Fact]
    public async Task TrackShowAsync_Finds_Exact_Show_Match()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "12345",
                    Title: "Andor",
                    Type: TrackedTitleType.Show,
                    Year: 2022)
                ])
        };

        var provider = new TraktTitleTrackingProvider(client);

        var result = await provider.TrackShowAsync("Andor");

        Assert.Equal("12345", result.ProviderId);
        Assert.Equal("Andor", result.Title);
        Assert.Equal(TrackedTitleType.Show, result.Type);
    }

    [Fact]
    public async Task TrackShowAsync_Ignores_Movie_Matches()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "movie-1",
                    Title: "Andor",
                    Type: TrackedTitleType.Movie,
                    Year: 2022)
                ])
        };

        var provider = new TraktTitleTrackingProvider(client);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.TrackShowAsync("Andor"));
    }

    [Fact]
    public async Task TrackShowAsync_Throws_When_Show_Not_Found()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>([])
        };

        var provider = new TraktTitleTrackingProvider(client);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.TrackShowAsync("Andor"));
    }

    [Fact]
    public async Task TrackShowAsync_Populates_TrackedTitle()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "12345",
                    Title: "Andor",
                    Type: TrackedTitleType.Show,
                    Year: 2022)
                ])
        };

        var provider = new TraktTitleTrackingProvider(client);

        var result = await provider.TrackShowAsync("Andor");

        Assert.Equal("12345", result.ProviderId);
        Assert.Equal("Andor", result.Title);
        Assert.Equal(TrackedTitleType.Show, result.Type);
    }

    [Fact]
    public async Task TrackShowAsync_Preserves_Platform()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "12345",
                    Title: "Andor",
                    Type: TrackedTitleType.Show,
                    Year: 2022)
                ])
        };

        var provider = new TraktTitleTrackingProvider(client);

        var result = await provider.TrackShowAsync("Andor", "Disney Plus");

        Assert.Equal("Disney Plus", result.Platform);
    }

    [Fact]
    public async Task TrackMovieAsync_Finds_Exact_Movie_Match()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "654321",
                    Title: "Dune: Part Two",
                    Type: TrackedTitleType.Movie,
                    Year: 2024)
                ])
        };

        var provider = new TraktTitleTrackingProvider(client);

        var result = await provider.TrackMovieAsync("Dune: Part Two");

        Assert.Equal("654321", result.ProviderId);
        Assert.Equal("Dune: Part Two", result.Title);
        Assert.Equal(TrackedTitleType.Movie, result.Type);
    }

    [Fact]
    public async Task TrackMovieAsync_Ignores_Show_Matches()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "show-1",
                    Title: "Dune: Part Two",
                    Type: TrackedTitleType.Show,
                    Year: 2024)
                ])
        };

        var provider = new TraktTitleTrackingProvider(client);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.TrackMovieAsync("Dune: Part Two"));
    }

    [Fact]
    public async Task TrackMovieAsync_Throws_When_Movie_Not_Found()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>([])
        };

        var provider = new TraktTitleTrackingProvider(client);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.TrackMovieAsync("Dune: Part Two"));
    }

    [Fact]
    public async Task TrackMovieAsync_Populates_TrackedTitle()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "654321",
                    Title: "Dune: Part Two",
                    Type: TrackedTitleType.Movie,
                    Year: 2024)
                ])
        };

        var provider = new TraktTitleTrackingProvider(client);

        var result = await provider.TrackMovieAsync("Dune: Part Two");

        Assert.Equal("654321", result.ProviderId);
        Assert.Equal("Dune: Part Two", result.Title);
        Assert.Equal(TrackedTitleType.Movie, result.Type);
    }

    [Fact]
    public async Task TrackMovieAsync_Preserves_Platform()
    {
        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "654321",
                    Title: "Dune: Part Two",
                    Type: TrackedTitleType.Movie,
                    Year: 2024)
                ])
        };

        var provider = new TraktTitleTrackingProvider(client);

        var result = await provider.TrackMovieAsync("Dune: Part Two", "Max");

        Assert.Equal("Max", result.Platform);
    }

    [Fact]
    public async Task TrackShowAsync_Trims_Title()
    {
        string? capturedQuery = null;

        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (query, _) =>
            {
                capturedQuery = query;

                return Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "12345",
                    Title: "Andor",
                    Type: TrackedTitleType.Show,
                    Year: 2022)
                ]);
            }
        };

        var provider = new TraktTitleTrackingProvider(client);

        await provider.TrackShowAsync("  Andor  ");

        Assert.Equal("Andor", capturedQuery);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task TrackShowAsync_Rejects_Blank_Title(string showTitle)
    {
        var client = new TestTraktTitleSearchClient();
        var provider = new TraktTitleTrackingProvider(client);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            provider.TrackShowAsync(showTitle));
    }

    [Fact]
    public async Task TrackMovieAsync_Trims_Title()
    {
        string? capturedQuery = null;

        var client = new TestTraktTitleSearchClient
        {
            SearchTitlesAsyncHandler = (query, _) =>
            {
                capturedQuery = query;

                return Task.FromResult<IReadOnlyList<TitleSearchResult>>(
                [
                    new(
                    ProviderId: "654321",
                    Title: "Dune: Part Two",
                    Type: TrackedTitleType.Movie,
                    Year: 2024)
                ]);
            }
        };

        var provider = new TraktTitleTrackingProvider(client);

        await provider.TrackMovieAsync("  Dune: Part Two  ");

        Assert.Equal("Dune: Part Two", capturedQuery);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task TrackMovieAsync_Rejects_Blank_Title(string movieTitle)
    {
        var client = new TestTraktTitleSearchClient();
        var provider = new TraktTitleTrackingProvider(client);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            provider.TrackMovieAsync(movieTitle));
    }

    private sealed class TestTraktTitleSearchClient : ITraktTitleSearchClient
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

        public Task<IReadOnlyList<TitleSearchResult>> SearchShowsAsync(
    string query,
    CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TitleSearchResult>> SearchMoviesAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}