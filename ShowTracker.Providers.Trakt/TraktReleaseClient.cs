using ShowTracker.Domain.Models;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ShowTracker.Providers.Trakt;

public sealed class TraktReleaseClient : ITraktReleaseClient
{
    private readonly HttpClient _httpClient;
    private readonly TraktOptions _options;

    public TraktReleaseClient(
        HttpClient httpClient,
        TraktOptions options)
    {
        _httpClient = httpClient
            ?? throw new ArgumentNullException(nameof(httpClient));

        _options = options
            ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
        CancellationToken cancellationToken = default)
    {
        ValidateClientId();

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var days = ValidateAndGetUpcomingReleaseDays();

        var showReleases = await GetShowReleasesAsync(
            startDate,
            days,
            cancellationToken);

        var movieReleases = await GetMovieReleasesAsync(
            startDate,
            days,
            cancellationToken);

        return showReleases
            .Concat(movieReleases)
            .OrderBy(release => release.ReleaseDate)
            .ThenBy(release => release.Title, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public async Task<UpcomingRelease?> GetNextReleaseAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        var normalizedTitle = title.Trim();

        var releases = await GetUpcomingReleasesAsync(cancellationToken);

        return releases.FirstOrDefault(release =>
            string.Equals(
                release.Title,
                normalizedTitle,
                StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IReadOnlyList<UpcomingRelease>> GetShowReleasesAsync(
        DateOnly startDate,
        int days,
        CancellationToken cancellationToken)
    {
        using var request = CreateRequest(
            $"calendars/all/shows/{startDate:yyyy-MM-dd}/{days}");

        var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var releases = await response.Content.ReadFromJsonAsync<List<TraktShowCalendarRelease>>(
            cancellationToken: cancellationToken);

        return releases?
            .Select(MapShowRelease)
            .Where(release => release is not null)
            .Cast<UpcomingRelease>()
            .ToArray() ?? [];
    }

    private async Task<IReadOnlyList<UpcomingRelease>> GetMovieReleasesAsync(
        DateOnly startDate,
        int days,
        CancellationToken cancellationToken)
    {
        using var request = CreateRequest(
            $"calendars/all/movies/{startDate:yyyy-MM-dd}/{days}");

        var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var releases = await response.Content.ReadFromJsonAsync<List<TraktMovieCalendarRelease>>(
            cancellationToken: cancellationToken);

        return releases?
            .Select(MapMovieRelease)
            .Where(release => release is not null)
            .Cast<UpcomingRelease>()
            .ToArray() ?? [];
    }

    private HttpRequestMessage CreateRequest(
        string relativePath)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri(
                _options.BaseAddress,
                relativePath));

        request.Headers.TryAddWithoutValidation("trakt-api-version", "2");
        request.Headers.TryAddWithoutValidation("trakt-api-key", _options.ClientId);
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("User-Agent", _options.UserAgent);

        return request;
    }

    private void ValidateClientId()
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId))
            throw new InvalidOperationException("Trakt client id is required.");
    }

    private int ValidateAndGetUpcomingReleaseDays()
    {
        if (_options.UpcomingReleaseDays < 1)
            throw new InvalidOperationException("Upcoming release days must be greater than zero.");

        return _options.UpcomingReleaseDays;
    }

    private static UpcomingRelease? MapShowRelease(
        TraktShowCalendarRelease release)
    {
        if (release.Show is null)
            return null;

        if (string.IsNullOrWhiteSpace(release.Show.Title))
            return null;

        if (!TryReadDate(release.FirstAired, out var releaseDate))
            return null;

        var providerId = release.Show.Ids?.Trakt?.ToString(CultureInfo.InvariantCulture)
            ?? release.Show.Title;

        if (string.IsNullOrWhiteSpace(providerId))
            return null;

        return new UpcomingRelease(
            ProviderId: providerId,
            Title: release.Show.Title,
            Type: TrackedTitleType.Show,
            ReleaseDate: releaseDate,
            SeasonNumber: release.Episode?.Season,
            EpisodeNumber: release.Episode?.Number,
            EpisodeTitle: NullIfWhiteSpace(release.Episode?.Title));
    }

    private static UpcomingRelease? MapMovieRelease(
        TraktMovieCalendarRelease release)
    {
        if (release.Movie is null)
            return null;

        if (string.IsNullOrWhiteSpace(release.Movie.Title))
            return null;

        if (!TryReadDate(release.Released, out var releaseDate))
            return null;

        var providerId = release.Movie.Ids?.Trakt?.ToString(CultureInfo.InvariantCulture)
            ?? release.Movie.Title;

        if (string.IsNullOrWhiteSpace(providerId))
            return null;

        return new UpcomingRelease(
            ProviderId: providerId,
            Title: release.Movie.Title,
            Type: TrackedTitleType.Movie,
            ReleaseDate: releaseDate);
    }

    private static bool TryReadDate(
        string? value,
        out DateOnly date)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var dateTimeOffset))
        {
            date = DateOnly.FromDateTime(dateTimeOffset.UtcDateTime);
            return true;
        }

        if (!string.IsNullOrWhiteSpace(value) &&
            DateOnly.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date))
        {
            return true;
        }

        date = default;
        return false;
    }

    private static string? NullIfWhiteSpace(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    private sealed class TraktShowCalendarRelease
    {
        [JsonPropertyName("first_aired")]
        public string? FirstAired { get; set; }

        public TraktEpisode? Episode { get; set; }

        public TraktTitle? Show { get; set; }
    }

    private sealed class TraktMovieCalendarRelease
    {
        public string? Released { get; set; }

        public TraktTitle? Movie { get; set; }
    }

    private sealed class TraktEpisode
    {
        public int? Season { get; set; }

        public int? Number { get; set; }

        public string? Title { get; set; }
    }

    private sealed class TraktTitle
    {
        public string Title { get; set; } = "";

        public int? Year { get; set; }

        public TraktIds? Ids { get; set; } = new();
    }

    private sealed class TraktIds
    {
        public int? Trakt { get; set; }
    }
}