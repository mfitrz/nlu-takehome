namespace NLUTakehome.Repositories;

using Npgsql;
using NpgsqlTypes;

public class ScofflawRepository(NpgsqlDataSource db, ILogger<ScofflawRepository> logger) : IScofflawRepository
{
    public async Task<List<string>> GetAddressesWithViolationsSinceAsync(DateOnly sinceDate)
    {
        try
        {
            await using var conn = await db.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
                SELECT DISTINCT s.address
                FROM scofflaws s
                INNER JOIN violations v ON s.address = v.address
                WHERE v.violation_date >= @sinceDate", conn);

            cmd.Parameters.Add(new NpgsqlParameter("sinceDate", NpgsqlDbType.Date) { Value = sinceDate });

            await using var reader = await cmd.ExecuteReaderAsync();

            var addresses = new List<string>();
            while (await reader.ReadAsync())
                addresses.Add(reader.GetString(0));

            return addresses;
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex, "Database error fetching scofflaw addresses since {SinceDate}", sinceDate);
            throw;
        }
    }
}
