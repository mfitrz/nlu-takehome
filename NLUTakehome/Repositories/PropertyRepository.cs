namespace NLUTakehome.Repositories;

using Npgsql;
using NLUTakehome.Models.Db;

public class PropertyRepository(NpgsqlDataSource db, ILogger<PropertyRepository> logger) : IPropertyRepository
{
    public async Task<(List<ViolationRow> Violations, bool IsScofflaw)> GetViolationsAsync(string address)
    {
        try
        {
            await using var conn = await db.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
                SELECT
                    v.violation_date,
                    v.violation_code,
                    v.violation_status,
                    v.violation_desc,
                    v.violation_inspc_comms,
                    s.address IS NOT NULL AS is_scofflaw
                FROM violations v
                LEFT JOIN scofflaws s ON v.address = s.address
                WHERE v.address = @address
                ORDER BY v.violation_date DESC", conn);

            cmd.Parameters.AddWithValue("address", address);

            await using var reader = await cmd.ExecuteReaderAsync();

            var violations = new List<ViolationRow>();
            var isScofflaw = false;

            while (await reader.ReadAsync())
            {
                violations.Add(new ViolationRow(
                    reader.IsDBNull(0) ? DateOnly.MinValue : DateOnly.FromDateTime(reader.GetDateTime(0)),
                    reader.IsDBNull(1) ? null : reader.GetString(1),
                    reader.IsDBNull(2) ? null : reader.GetString(2),
                    reader.IsDBNull(3) ? null : reader.GetString(3),
                    reader.IsDBNull(4) ? null : reader.GetString(4)
                ));

                isScofflaw = reader.GetBoolean(5);
            }

            return (violations, isScofflaw);
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex, "Database error in GET /property/{Address}", address);
            throw;
        }
    }
}
