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
        var client = CreateClient([]);

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
        var capturedRequests = new List<HttpRequestMessage>();

        var httpClient = new HttpClient(new TestHttpMessageHandler(request =>
        {
            capturedRequests.Add(request);

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

        Assert.Equal(2, capturedRequests.Count);

        foreach (var request in capturedRequests)
        {
            Assert.True(request.Headers.TryGetValues("trakt-api-version", out var versionValues));
            Assert.Equal("2", Assert.Single(versionValues));

            Assert.True(request.Headers.TryGetValues("trakt-api-key", out var apiKeyValues));
            Assert.Equal("test-client-id", Assert.Single(apiKeyValues));

            Assert.True(request.Headers.TryGetValues("Accept", out var acceptValues));
            Assert.Contains("application/json", acceptValues);
        }
    }

    [Fact]
    public async Task SearchTitlesAsync_Uses_Show_And_Movie_Search_Endpoints_With_Escaped_Query()
    {
        var capturedRequests = new List<HttpRequestMessage>();

        var httpClient = new HttpClient(new TestHttpMessageHandler(request =>
        {
            capturedRequests.Add(request);

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

        await client.SearchTitlesAsync("Your Friends and Neighbors");

        Assert.Equal(2, capturedRequests.Count);

        Assert.Contains(
            capturedRequests,
            request => string.Equals(
                request.RequestUri!.AbsoluteUri,
                "https://api.trakt.tv/search/show?query=Your%20Friends%20and%20Neighbors",
                StringComparison.Ordinal));

        Assert.Contains(
            capturedRequests,
            request => string.Equals(
                request.RequestUri!.AbsoluteUri,
                "https://api.trakt.tv/search/movie?query=Your%20Friends%20and%20Neighbors",
                StringComparison.Ordinal));
    }

    [Fact]
    public async Task SearchTitlesAsync_Maps_Show_Result()
    {
        var client = CreateClient(
        [
            """
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
            """,
            "[]"
        ]);

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
        var client = CreateClient(
        [
            "[]",
            """
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
            """
        ]);

        var results = await client.SearchTitlesAsync("Dune Part Two");

        var result = Assert.Single(results);
        Assert.Equal("654321", result.ProviderId);
        Assert.Equal("Dune: Part Two", result.Title);
        Assert.Equal(TrackedTitleType.Movie, result.Type);
        Assert.Equal(2024, result.Year);
        Assert.Null(result.Platform);
    }

    [Fact]
    public async Task SearchTitlesAsync_Returns_Combined_Show_And_Movie_Results()
    {
        var client = CreateClient(
        [
            """
            [
              {
                "type": "show",
                "show": {
                  "title": "Andor",
                  "year": 2022,
                  "ids": {
                    "trakt": 12345
                  }
                }
              }
            ]
            """,
            """
            [
              {
                "type": "movie",
                "movie": {
                  "title": "Dune: Part Two",
                  "year": 2024,
                  "ids": {
                    "trakt": 654321
                  }
                }
              }
            ]
            """
        ]);

        var results = await client.SearchTitlesAsync("andor");

        Assert.Equal(2, results.Count);
        Assert.Contains(results, result => result.Type == TrackedTitleType.Show);
        Assert.Contains(results, result => result.Type == TrackedTitleType.Movie);
    }

    [Fact]
    public async Task SearchTitlesAsync_Returns_Empty_List_When_Response_Is_Empty()
    {
        var client = CreateClient(
        [
            "[]",
            "[]"
        ]);

        var results = await client.SearchTitlesAsync("Definitely Missing");

        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchTitlesAsync_Maps_Search_Score()
    {
        var client = CreateClient(
        [
            """
        [
          {
            "type": "show",
            "score": 987.65,
            "show": {
              "title": "FROM",
              "year": 2022,
              "ids": {
                "trakt": 12345
              }
            }
          }
        ]
        """,
        "[]"
        ]);

        var results = await client.SearchTitlesAsync("FROM");

        var result = Assert.Single(results);
        Assert.Equal(987.65, result.Score);
    }

    [Fact]
    public async Task SearchTitlesAsync_Orders_Exact_Title_Matches_First_Then_By_Score()
    {
        var client = CreateClient(
        [
            """
        [
          {
            "type": "show",
            "score": 100,
            "show": {
              "title": "Stories From Somewhere",
              "year": 2020,
              "ids": {
                "trakt": 1
              }
            }
          },
          {
            "type": "show",
            "score": 50,
            "show": {
              "title": "FROM",
              "year": 2022,
              "ids": {
                "trakt": 2
              }
            }
          },
          {
            "type": "show",
            "score": 75,
            "show": {
              "title": "From",
              "year": 2022,
              "ids": {
                "trakt": 3
              }
            }
          }
        ]
        """,
        """
        [
          {
            "type": "movie",
            "score": 200,
            "movie": {
              "title": "A Movie From Mars",
              "year": 2019,
              "ids": {
                "trakt": 4
              }
            }
          }
        ]
        """
        ]);

        var results = await client.SearchTitlesAsync("FROM");

        Assert.Equal(4, results.Count);
        Assert.Equal("From", results[0].Title);
        Assert.Equal("FROM", results[1].Title);
        Assert.Equal("A Movie From Mars", results[2].Title);
        Assert.Equal("Stories From Somewhere", results[3].Title);
    }

    [Fact]
    public async Task SearchTitlesAsync_Limits_Results_To_Top_Ten()
    {
        var showJson = """
        [
          { "type": "show", "score": 20, "show": { "title": "Show 01", "year": 2020, "ids": { "trakt": 1 } } },
          { "type": "show", "score": 19, "show": { "title": "Show 02", "year": 2020, "ids": { "trakt": 2 } } },
          { "type": "show", "score": 18, "show": { "title": "Show 03", "year": 2020, "ids": { "trakt": 3 } } },
          { "type": "show", "score": 17, "show": { "title": "Show 04", "year": 2020, "ids": { "trakt": 4 } } },
          { "type": "show", "score": 16, "show": { "title": "Show 05", "year": 2020, "ids": { "trakt": 5 } } },
          { "type": "show", "score": 15, "show": { "title": "Show 06", "year": 2020, "ids": { "trakt": 6 } } },
          { "type": "show", "score": 14, "show": { "title": "Show 07", "year": 2020, "ids": { "trakt": 7 } } },
          { "type": "show", "score": 13, "show": { "title": "Show 08", "year": 2020, "ids": { "trakt": 8 } } },
          { "type": "show", "score": 12, "show": { "title": "Show 09", "year": 2020, "ids": { "trakt": 9 } } },
          { "type": "show", "score": 11, "show": { "title": "Show 10", "year": 2020, "ids": { "trakt": 10 } } },
          { "type": "show", "score": 10, "show": { "title": "Show 11", "year": 2020, "ids": { "trakt": 11 } } }
        ]
        """;

        var client = CreateClient(
        [
            showJson,
        "[]"
        ]);

        var results = await client.SearchTitlesAsync("Show");

        Assert.Equal(10, results.Count);
        Assert.DoesNotContain(results, result => result.Title == "Show 11");
    }

    private static TraktTitleSearchClient CreateClient(
        IReadOnlyList<string> responseJson)
    {
        var responseIndex = 0;

        var httpClient = new HttpClient(new TestHttpMessageHandler(_ =>
        {
            var content = responseIndex < responseJson.Count
                ? responseJson[responseIndex]
                : "[]";

            responseIndex++;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };
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

        public TestHttpMessageHandler(
            Func<HttpRequestMessage, HttpResponseMessage> handler)
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