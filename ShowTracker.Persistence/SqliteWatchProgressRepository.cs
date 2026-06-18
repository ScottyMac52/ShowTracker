using Microsoft.Data.Sqlite;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Persistence;

public sealed class SqliteWatchProgressRepository : IWatchProgressRepository
{
    private readonly SqliteConnection _connection;
    private bool _initialized;

    public SqliteWatchProgressRepository(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task SaveAsync(
        WatchProgress progress,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(progress);

        await EnsureCreatedAsync(cancellationToken);

        await using var command = _connection.CreateCommand();
        command.CommandText = """
            INSERT INTO WatchProgress
            (
                ShowTitle,
                ProviderId,
                LastWatchedSeason,
                LastWatchedEpisode,
                LastWatchedEpisodeTitle,
                NextSeason,
                NextEpisode,
                NextEpisodeTitle
            )
            VALUES
            (
                $showTitle,
                $providerId,
                $lastWatchedSeason,
                $lastWatchedEpisode,
                $lastWatchedEpisodeTitle,
                $nextSeason,
                $nextEpisode,
                $nextEpisodeTitle
            )
            ON CONFLICT(ShowTitle) DO UPDATE SET
                ProviderId = excluded.ProviderId,
                LastWatchedSeason = excluded.LastWatchedSeason,
                LastWatchedEpisode = excluded.LastWatchedEpisode,
                LastWatchedEpisodeTitle = excluded.LastWatchedEpisodeTitle,
                NextSeason = excluded.NextSeason,
                NextEpisode = excluded.NextEpisode,
                NextEpisodeTitle = excluded.NextEpisodeTitle;
            """;

        command.Parameters.AddWithValue("$showTitle", progress.ShowTitle);
        command.Parameters.AddWithValue("$providerId", progress.ProviderId);
        command.Parameters.AddWithValue("$lastWatchedSeason", (object?)progress.LastWatchedSeason ?? DBNull.Value);
        command.Parameters.AddWithValue("$lastWatchedEpisode", (object?)progress.LastWatchedEpisode ?? DBNull.Value);
        command.Parameters.AddWithValue("$lastWatchedEpisodeTitle", (object?)progress.LastWatchedEpisodeTitle ?? DBNull.Value);
        command.Parameters.AddWithValue("$nextSeason", (object?)progress.NextSeason ?? DBNull.Value);
        command.Parameters.AddWithValue("$nextEpisode", (object?)progress.NextEpisode ?? DBNull.Value);
        command.Parameters.AddWithValue("$nextEpisodeTitle", (object?)progress.NextEpisodeTitle ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<WatchProgress?> GetAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        await EnsureCreatedAsync(cancellationToken);

        await using var command = _connection.CreateCommand();
        command.CommandText = """
            SELECT
                ProviderId,
                ShowTitle,
                LastWatchedSeason,
                LastWatchedEpisode,
                LastWatchedEpisodeTitle,
                NextSeason,
                NextEpisode,
                NextEpisodeTitle
            FROM WatchProgress
            WHERE ShowTitle = $showTitle;
            """;

        command.Parameters.AddWithValue("$showTitle", showTitle);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return ReadWatchProgress(reader);
    }

    private async Task EnsureCreatedAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
            return;

        if (_connection.State != System.Data.ConnectionState.Open)
            await _connection.OpenAsync(cancellationToken);

        await using var command = _connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS WatchProgress
            (
                ShowTitle TEXT NOT NULL PRIMARY KEY,
                ProviderId TEXT NOT NULL,
                LastWatchedSeason INTEGER NULL,
                LastWatchedEpisode INTEGER NULL,
                LastWatchedEpisodeTitle TEXT NULL,
                NextSeason INTEGER NULL,
                NextEpisode INTEGER NULL,
                NextEpisodeTitle TEXT NULL
            );
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);

        _initialized = true;
    }

    private static WatchProgress ReadWatchProgress(SqliteDataReader reader)
    {
        return new WatchProgress(
            ProviderId: reader.GetString(0),
            ShowTitle: reader.GetString(1),
            LastWatchedSeason: reader.IsDBNull(2) ? null : reader.GetInt32(2),
            LastWatchedEpisode: reader.IsDBNull(3) ? null : reader.GetInt32(3),
            LastWatchedEpisodeTitle: reader.IsDBNull(4) ? null : reader.GetString(4),
            NextSeason: reader.IsDBNull(5) ? null : reader.GetInt32(5),
            NextEpisode: reader.IsDBNull(6) ? null : reader.GetInt32(6),
            NextEpisodeTitle: reader.IsDBNull(7) ? null : reader.GetString(7));
    }
}