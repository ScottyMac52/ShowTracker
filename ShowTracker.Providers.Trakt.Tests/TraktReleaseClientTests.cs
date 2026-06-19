using System.Net;
using ShowTracker.Domain.Models;

namespace ShowTracker.Providers.Trakt.Tests;

public sealed class TraktReleaseClientTests
{
    [Fact]
    public async Task GetUpcomingReleasesAsync_Rejects_Missing_Client_Id()
    {
        var httpClient = new HttpClient(new TestHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            }));

        var client = new TraktReleaseClient(
            httpClient,
            new TraktOptions
            {
                ClientId = ""
            });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            client.GetUpcomingReleasesAsync());
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Sends_Required_Trakt_Headers()
    {
        var capturedRequests = new List<HttpRequestMessage>();

        var client = CreateClient(
        [
            "[]",
            "[]"
        ],
        capturedRequests);

        await client.GetUpcomingReleasesAsync();

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
    public async Task GetUpcomingReleasesAsync_Uses_Show_And_Movie_Calendar_Endpoints()
    {
        var capturedRequests = new List<HttpRequestMessage>();

        var client = CreateClient(
        [
            "[]",
            "[]"
        ],
        capturedRequests,
        upcomingReleaseDays: 7);

        await client.GetUpcomingReleasesAsync();

        Assert.Equal(2, capturedRequests.Count);

        Assert.Contains(
            capturedRequests,
            request =>
                request.RequestUri!.AbsolutePath.StartsWith(
                    "/calendars/all/shows/",
                    StringComparison.Ordinal) &&
                request.RequestUri.AbsolutePath.EndsWith(
                    "/7",
                    StringComparison.Ordinal));

        Assert.Contains(
            capturedRequests,
            request =>
                request.RequestUri!.AbsolutePath.StartsWith(
                    "/calendars/all/movies/",
                    StringComparison.Ordinal) &&
                request.RequestUri.AbsolutePath.EndsWith(
                    "/7",
                    StringComparison.Ordinal));
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Maps_Show_Releases()
    {
        var client = CreateClient(
        [
            """
            [
              {
                "first_aired": "2026-06-20T01:00:00.000Z",
                "episode": {
                  "season": 5,
                  "number": 1,
                  "title": "Episode One"
                },
                "show": {
                  "title": "The Boys",
                  "year": 2019,
                  "ids": {
                    "trakt": 139960
                  }
                }
              }
            ]
            """,
            "[]"
        ]);

        var releases = await client.GetUpcomingReleasesAsync();

        var release = Assert.Single(releases);
        Assert.Equal("139960", release.ProviderId);
        Assert.Equal("The Boys", release.Title);
        Assert.Equal(TrackedTitleType.Show, release.Type);
        Assert.Equal(new DateOnly(2026, 6, 20), release.ReleaseDate);
        Assert.Equal(5, release.SeasonNumber);
        Assert.Equal(1, release.EpisodeNumber);
        Assert.Equal("Episode One", release.EpisodeTitle);
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Maps_Movie_Releases()
    {
        var client = CreateClient(
        [
            "[]",
            """
            [
              {
                "released": "2026-06-21",
                "movie": {
                  "title": "Dune: Part Three",
                  "year": 2026,
                  "ids": {
                    "trakt": 987654
                  }
                }
              }
            ]
            """
        ]);

        var releases = await client.GetUpcomingReleasesAsync();

        var release = Assert.Single(releases);
        Assert.Equal("987654", release.ProviderId);
        Assert.Equal("Dune: Part Three", release.Title);
        Assert.Equal(TrackedTitleType.Movie, release.Type);
        Assert.Equal(new DateOnly(2026, 6, 21), release.ReleaseDate);
        Assert.Null(release.SeasonNumber);
        Assert.Null(release.EpisodeNumber);
        Assert.Null(release.EpisodeTitle);
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Returns_Combined_Show_And_Movie_Releases()
    {
        var client = CreateClient(
        [
            """
            [
              {
                "first_aired": "2026-06-20T01:00:00.000Z",
                "episode": {
                  "season": 5,
                  "number": 1,
                  "title": "Episode One"
                },
                "show": {
                  "title": "The Boys",
                  "ids": {
                    "trakt": 139960
                  }
                }
              }
            ]
            """,
            """
            [
              {
                "released": "2026-06-21",
                "movie": {
                  "title": "Dune: Part Three",
                  "ids": {
                    "trakt": 987654
                  }
                }
              }
            ]
            """
        ]);

        var releases = await client.GetUpcomingReleasesAsync();

        Assert.Equal(2, releases.Count);
        Assert.Contains(releases, release => release.Type == TrackedTitleType.Show);
        Assert.Contains(releases, release => release.Type == TrackedTitleType.Movie);
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Orders_Releases_By_Date_Then_Title()
    {
        var client = CreateClient(
        [
            """
            [
              {
                "first_aired": "2026-06-22T01:00:00.000Z",
                "episode": {
                  "season": 1,
                  "number": 2,
                  "title": "Second"
                },
                "show": {
                  "title": "Zulu Show",
                  "ids": {
                    "trakt": 2
                  }
                }
              },
              {
                "first_aired": "2026-06-20T01:00:00.000Z",
                "episode": {
                  "season": 1,
                  "number": 1,
                  "title": "First"
                },
                "show": {
                  "title": "Alpha Show",
                  "ids": {
                    "trakt": 1
                  }
                }
              }
            ]
            """,
            """
            [
              {
                "released": "2026-06-20",
                "movie": {
                  "title": "Beta Movie",
                  "ids": {
                    "trakt": 3
                  }
                }
              }
            ]
            """
        ]);

        var releases = await client.GetUpcomingReleasesAsync();

        Assert.Equal("Alpha Show", releases[0].Title);
        Assert.Equal("Beta Movie", releases[1].Title);
        Assert.Equal("Zulu Show", releases[2].Title);
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Returns_Empty_List_When_Calendars_Are_Empty()
    {
        var client = CreateClient(
        [
            "[]",
            "[]"
        ]);

        var releases = await client.GetUpcomingReleasesAsync();

        Assert.Empty(releases);
    }

    [Fact]
    public async Task GetNextReleaseAsync_Returns_Earliest_Matching_Title()
    {
        var client = CreateClient(
        [
            """
            [
              {
                "first_aired": "2026-06-25T01:00:00.000Z",
                "episode": {
                  "season": 5,
                  "number": 2,
                  "title": "Second"
                },
                "show": {
                  "title": "The Boys",
                  "ids": {
                    "trakt": 139960
                  }
                }
              },
              {
                "first_aired": "2026-06-20T01:00:00.000Z",
                "episode": {
                  "season": 5,
                  "number": 1,
                  "title": "First"
                },
                "show": {
                  "title": "The Boys",
                  "ids": {
                    "trakt": 139960
                  }
                }
              }
            ]
            """,
            "[]"
        ]);

        var release = await client.GetNextReleaseAsync("The Boys");

        Assert.NotNull(release);
        Assert.Equal("The Boys", release.Title);
        Assert.Equal(new DateOnly(2026, 6, 20), release.ReleaseDate);
        Assert.Equal(1, release.EpisodeNumber);
    }

    [Fact]
    public async Task GetNextReleaseAsync_Returns_Null_When_Title_Does_Not_Match()
    {
        var client = CreateClient(
        [
            """
            [
              {
                "first_aired": "2026-06-20T01:00:00.000Z",
                "episode": {
                  "season": 5,
                  "number": 1,
                  "title": "First"
                },
                "show": {
                  "title": "The Boys",
                  "ids": {
                    "trakt": 139960
                  }
                }
              }
            ]
            """,
            "[]"
        ]);

        var release = await client.GetNextReleaseAsync("Andor");

        Assert.Null(release);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetNextReleaseAsync_Rejects_Blank_Title(
        string title)
    {
        var client = CreateClient(
        [
            "[]",
            "[]"
        ]);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetNextReleaseAsync(title));
    }

    [Fact]
    public async Task GetUpcomingReleasesAsync_Rejects_Invalid_UpcomingReleaseDays()
    {
        var client = CreateClient(
        [
            "[]",
            "[]"
        ],
        upcomingReleaseDays: 0);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            client.GetUpcomingReleasesAsync());
    }

    private static TraktReleaseClient CreateClient(
        IReadOnlyList<string> responseJson,
        List<HttpRequestMessage>? capturedRequests = null,
        int? upcomingReleaseDays = null)
    {
        var responseIndex = 0;

        var httpClient = new HttpClient(new TestHttpMessageHandler(request =>
        {
            capturedRequests?.Add(request);

            var content = responseIndex < responseJson.Count
                ? responseJson[responseIndex]
                : "[]";

            responseIndex++;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };
        }));

        return new TraktReleaseClient(
            httpClient,
            new TraktOptions
            {
                ClientId = "test-client-id",
                UserAgent = "ShowTracker.Tests/1.0",
                UpcomingReleaseDays = upcomingReleaseDays ?? 30
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