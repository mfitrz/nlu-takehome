namespace NLUTakehome.Ingestion.Repositories;

using Npgsql;
using NLUTakehome.Ingestion.Models.Parsed;

public class ViolationsRepository(NpgsqlConnection connection)
{
    public async Task InsertBatchAsync(List<ParsedViolation> batch)
    {
        var placeholders = new List<string>(batch.Count);
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;

        for (int i = 0; i < batch.Count; i++)
        {
            var v = batch[i];
            placeholders.Add($"(@id{i}, @address{i}, @date{i}, @code{i}, @status{i}, @desc{i}, @comments{i})");
            cmd.Parameters.AddWithValue($"id{i}", v.Id);
            cmd.Parameters.AddWithValue($"address{i}", v.Address);
            cmd.Parameters.AddWithValue($"date{i}", v.ViolationDate.HasValue ? v.ViolationDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue($"code{i}", v.Code ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue($"status{i}", v.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue($"desc{i}", v.Desc ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue($"comments{i}", v.InspectorComments ?? (object)DBNull.Value);
        }

        cmd.CommandText = $@"
            INSERT INTO violations (id, address, violation_date, violation_code, violation_status, violation_desc, violation_inspc_comms)
            VALUES {string.Join(", ", placeholders)}
            ON CONFLICT (id) DO NOTHING";

        await cmd.ExecuteNonQueryAsync();
    }
}
