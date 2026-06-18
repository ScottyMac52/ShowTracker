using System.Net.Http.Json;
using ShowTracker.Domain.Models;

namespace ShowTracker.Providers.Trakt;

public sealed class TraktTitleSearchClient : ITraktTitleSearchClient
{
    private readonly HttpClient _httpClient;
    private readonly TraktOptions _options;

    public TraktTitleSearchClient(
        HttpClient httpClient,
        TraktOptions options)
    {
        _httpClient = httpClient
            ?? throw new ArgumentNullException(nameof(httpClient));

        _options = options
            ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Search query is required.", nameof(query));

        if (string.IsNullOrWhiteSpace(_options.ClientId))
            throw new InvalidOperationException("Trakt client id is required.");

        var normalizedQuery = query.Trim();

        var showResults = await SearchAsync(
            "show",
            normalizedQuery,
            cancellationToken);

        var movieResults = await SearchAsync(
            "movie",
            normalizedQuery,
            cancellationToken);

        return showResults
            .Concat(movieResults)
            .OrderByDescending(result =>
                string.Equals(result.Title, normalizedQuery, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(result => result.Score ?? 0)
            .Take(10)
            .ToArray();
    }

    private async Task<IReadOnlyList<TitleSearchResult>> SearchAsync(
        string type,
        string query,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri(
                _options.BaseAddress,
                $"search/{type}?query={Uri.EscapeDataString(query)}"));

        request.Headers.TryAddWithoutValidation("trakt-api-version", "2");
        request.Headers.TryAddWithoutValidation("trakt-api-key", _options.ClientId);
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("User-Agent", _options.UserAgent);

        var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync<List<TraktSearchResult>>(
            cancellationToken: cancellationToken);

        return results?
            .Select(Map)
            .Where(result => result is not null)
            .Cast<TitleSearchResult>()
            .ToArray() ?? [];
    }

    private static TitleSearchResult? Map(
        TraktSearchResult result)
    {
        if (result.Show is not null)
        {
            return new TitleSearchResult(
                ProviderId: result.Show.Ids.Trakt?.ToString() ?? result.Show.Title,
                Title: result.Show.Title,
                Type: TrackedTitleType.Show,
                Year: result.Show.Year,
                Score: result.Score);
        }

        if (result.Movie is not null)
        {
            return new TitleSearchResult(
                ProviderId: result.Movie.Ids.Trakt?.ToString() ?? result.Movie.Title,
                Title: result.Movie.Title,
                Type: TrackedTitleType.Movie,
                Year: result.Movie.Year,
                Score: result.Score);
        }

        return null;
    }

    private sealed class TraktSearchResult
    {
        public string Type { get; set; } = "";
        public double? Score { get; set; }
        public TraktTitle? Show { get; set; }
        public TraktTitle? Movie { get; set; }
    }

    private sealed class TraktTitle
    {
        public string Title { get; set; } = "";
        public int? Year { get; set; }
        public TraktIds Ids { get; set; } = new();
    }

    private sealed class TraktIds
    {
        public int? Trakt { get; set; }
    }
}