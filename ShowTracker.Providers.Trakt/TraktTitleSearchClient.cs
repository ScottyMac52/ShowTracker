using System.Net.Http.Json;
using ShowTracker.Domain.Models;

namespace ShowTracker.Providers.Trakt;

/// <summary>
/// Implements <see cref="ITraktTitleSearchClient"/> to search for titles using the Trakt API.
/// </summary>
/// <remarks>
/// Constructor for the TraktTitleSearchClient.
/// </remarks>
/// <param name="httpClient"><see cref="HttpClient"/> to be used for calls</param>
/// <param name="options"><see cref="TraktOptions"/> for the http call</param>
/// <exception cref="ArgumentNullException"><see cref="HttpClient"/> and <see cref="TraktOptions"/> are both required</exception>
public sealed class TraktTitleSearchClient(HttpClient httpClient, TraktOptions options) : ITraktTitleSearchClient
{
    #region Fields
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly TraktOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    #endregion Fields

    /// <summary>
    /// Searches for titles using the Trakt API.
    /// </summary>
    /// <param name="query">Query to use to use in the search</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Search query is required</exception>
    /// <exception cref="InvalidOperationException">Trakt client id is required</exception>
    public async Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Search query is required.", nameof(query));

        if (string.IsNullOrWhiteSpace(_options.ClientId))
            throw new InvalidOperationException("Trakt client id is required.");

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri(_options.BaseAddress, $"search/movie,show?query={Uri.EscapeDataString(query.Trim())}"));

        request.Headers.TryAddWithoutValidation("trakt-api-version", "2");
        request.Headers.TryAddWithoutValidation("trakt-api-key", _options.ClientId);
        request.Headers.TryAddWithoutValidation("Accept", "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync<List<TraktSearchResult>>(
            cancellationToken: cancellationToken);

        return results?
            .Select(Map)
            .Where(r => r is not null)
            .Cast<TitleSearchResult>()
            .ToArray() ?? [];
    }

    #region Private helpers

    private static TitleSearchResult? Map(TraktSearchResult result)
    {
        if (result.Show is not null)
        {
            return new TitleSearchResult(
                ProviderId: result.Show.Ids.Trakt?.ToString() ?? result.Show.Title,
                Title: result.Show.Title,
                Type: TrackedTitleType.Show,
                Year: result.Show.Year);
        }

        if (result.Movie is not null)
        {
            return new TitleSearchResult(
                ProviderId: result.Movie.Ids.Trakt?.ToString() ?? result.Movie.Title,
                Title: result.Movie.Title,
                Type: TrackedTitleType.Movie,
                Year: result.Movie.Year);
        }

        return null;
    }

    private sealed class TraktSearchResult
    {
        public string Type { get; set; } = "";
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

    #endregion Private helpers
}