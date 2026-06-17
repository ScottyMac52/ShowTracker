using System.Net;
using ShowTracker.Domain.Models;

namespace ShowTracker.Providers.Trakt.Tests;

public sealed class TraktTitleSearchClientTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SearchTitlesAsync_Rejects_Blank_Query(string query)
    {
        var client = CreateClient("""
            []
            """);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.SearchTitlesAsync(query));
    }

    [Fact]
    public async Task SearchTitlesAsync_Rejects_Missing_Client_Id()
    {
        var httpClient = new HttpClient(new TestHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            }));

        var client = new TraktTitleSearchClient(
            httpClient,
            new TraktOptions
            {
                ClientId = ""
            });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            client.SearchTitlesAsync("Andor"));
    }

    [Fact]
    public async Task SearchTitlesAsync_Sends_Required_Trakt_Headers()
    {
        HttpRequestMessage? capturedRequest = null;

        var httpClient = new HttpClient(new TestHttpMessageHandler(request =>
        {
            capturedRequest = request;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            };
        }));

        var client = new TraktTitleSearchClient(
            httpClient,
            new TraktOptions
            {
                ClientId = "test-client-id"
            });

        await client.SearchTitlesAsync("Andor");

        Assert.NotNull(capturedRequest);
        Assert.True(capturedRequest!.Headers.TryGetValues("trakt-api-version", out var versionValues));
        Assert.Equal("2", Assert.Single(versionValues));

        Assert.True(capturedRequest.Headers.TryGetValues("trakt-api-key", out var apiKeyValues));
        Assert.Equal("test-client-id", Assert.Single(apiKeyValues));

        Assert.True(capturedRequest.Headers.Accept.Any(h =>
            string.Equals(h.MediaType, "application/json", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public async Task SearchTitlesAsync_Uses_Search_Endpoint_With_Escaped_Query()
    {
        HttpRequestMessage? capturedRequest = null;

        var httpClient = new HttpClient(new TestHttpMessageHandler(request =>
        {
            capturedRequest = request;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            };
        }));

        var client = new TraktTitleSearchClient(
            httpClient,
            new TraktOptions
            {
                ClientId = "test-client-id"
            });

        await client.SearchTitlesAsync("Your Friends & Neighbors");

        Assert.NotNull(capturedRequest);
        Assert.Equal(
            "https://api.trakt.tv/search/movie,show?query=Your%20Friends%20%26%20Neighbors",
            capturedRequest!.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task SearchTitlesAsync_Maps_Show_Result()
    {
        var client = CreateClient("""
            [
              {
                "type": "show",
                "score": 1000,
                "show": {
                  "title": "Andor",
                  "year": 2022,
                  "ids": {
                    "trakt": 12345,
                    "slug": "andor",
                    "tvdb": 393189,
                    "imdb": "tt9253284",
                    "tmdb": 83867
                  }
                }
              }
            ]
            """);

        var results = await client.SearchTitlesAsync("Andor");

        var result = Assert.Single(results);
        Assert.Equal("12345", result.ProviderId);
        Assert.Equal("Andor", result.Title);
        Assert.Equal(TrackedTitleType.Show, result.Type);
        Assert.Equal(2022, result.Year);
        Assert.Null(result.Platform);
    }

    [Fact]
    public async Task SearchTitlesAsync_Maps_Movie_Result()
    {
        var client = CreateClient("""
            [
              {
                "type": "movie",
                "score": 1000,
                "movie": {
                  "title": "Dune: Part Two",
                  "year": 2024,
                  "ids": {
                    "trakt": 654321,
                    "slug": "dune-part-two-2024",
                    "imdb": "tt15239678",
                    "tmdb": 693134
                  }
                }
              }
            ]
            """);

        var results = await client.SearchTitlesAsync("Dune Part Two");

        var result = Assert.Single(results);
        Assert.Equal("654321", result.ProviderId);
        Assert.Equal("Dune: Part Two", result.Title);
        Assert.Equal(TrackedTitleType.Movie, result.Type);
        Assert.Equal(2024, result.Year);
        Assert.Null(result.Platform);
    }

    [Fact]
    public async Task SearchTitlesAsync_Returns_Empty_List_When_Response_Is_Empty()
    {
        var client = CreateClient("""
            []
            """);

        var results = await client.SearchTitlesAsync("Definitely Missing");

        Assert.Empty(results);
    }

    private static TraktTitleSearchClient CreateClient(string responseJson)
    {
        var httpClient = new HttpClient(new TestHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson)
            }));

        return new TraktTitleSearchClient(
            httpClient,
            new TraktOptions
            {
                ClientId = "test-client-id"
            });
    }

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}