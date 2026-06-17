using Microsoft.Data.Sqlite;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Persistence;

public sealed class SqliteTrackedTitleRepository : ITrackedTitleRepository
{
    private readonly SqliteConnection _connection;
    private bool _initialized;

    public SqliteTrackedTitleRepository(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task AddAsync(
        TrackedTitle trackedTitle,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trackedTitle);

        await EnsureCreatedAsync(cancellationToken);

        await using var command = _connection.CreateCommand();
        command.CommandText = """
            INSERT INTO TrackedTitles
            (
                ProviderId,
                Title,
                Type,
                Platform
            )
            VALUES
            (
                $providerId,
                $title,
                $type,
                $platform
            );
            """;

        command.Parameters.AddWithValue("$providerId", trackedTitle.ProviderId);
        command.Parameters.AddWithValue("$title", trackedTitle.Title);
        command.Parameters.AddWithValue("$type", trackedTitle.Type.ToString());
        command.Parameters.AddWithValue("$platform", (object?)trackedTitle.Platform ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<TrackedTitle?> FindByProviderIdAsync(
        string providerId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider id is required.", nameof(providerId));

        await EnsureCreatedAsync(cancellationToken);

        await using var command = _connection.CreateCommand();
        command.CommandText = """
            SELECT ProviderId, Title, Type, Platform
            FROM TrackedTitles
            WHERE ProviderId = $providerId;
            """;

        command.Parameters.AddWithValue("$providerId", providerId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return ReadTrackedTitle(reader);
    }

    public async Task<IReadOnlyList<TrackedTitle>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await EnsureCreatedAsync(cancellationToken);

        await using var command = _connection.CreateCommand();
        command.CommandText = """
            SELECT ProviderId, Title, Type, Platform
            FROM TrackedTitles
            ORDER BY Title;
            """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var results = new List<TrackedTitle>();

        while (await reader.ReadAsync(cancellationToken))
            results.Add(ReadTrackedTitle(reader));

        return results;
    }

    public async Task RemoveAsync(
        string providerId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider id is required.", nameof(providerId));

        await EnsureCreatedAsync(cancellationToken);

        await using var command = _connection.CreateCommand();
        command.CommandText = """
            DELETE FROM TrackedTitles
            WHERE ProviderId = $providerId;
            """;

        command.Parameters.AddWithValue("$providerId", providerId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task EnsureCreatedAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
            return;

        if (_connection.State != System.Data.ConnectionState.Open)
            await _connection.OpenAsync(cancellationToken);

        await using var command = _connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS TrackedTitles
            (
                ProviderId TEXT NOT NULL PRIMARY KEY,
                Title TEXT NOT NULL,
                Type TEXT NOT NULL,
                Platform TEXT NULL
            );
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);

        _initialized = true;
    }

    private static TrackedTitle ReadTrackedTitle(SqliteDataReader reader)
    {
        return new TrackedTitle(
            ProviderId: reader.GetString(0),
            Title: reader.GetString(1),
            Type: Enum.Parse<TrackedTitleType>(reader.GetString(2)),
            Platform: reader.IsDBNull(3) ? null : reader.GetString(3));
    }
}