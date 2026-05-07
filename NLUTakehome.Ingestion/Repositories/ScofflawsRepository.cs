namespace NLUTakehome.Ingestion.Repositories;

using Npgsql;

public class ScofflawsRepository(NpgsqlConnection connection)
{
    public async Task InsertBatchAsync(List<string> addresses)
    {
        var placeholders = new List<string>(addresses.Count);
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;

        for (int i = 0; i < addresses.Count; i++)
        {
            placeholders.Add($"(@address{i})");
            cmd.Parameters.AddWithValue($"address{i}", addresses[i]);
        }

        cmd.CommandText = $"INSERT INTO scofflaws (address) VALUES {string.Join(", ", placeholders)} ON CONFLICT (address) DO NOTHING";

        await cmd.ExecuteNonQueryAsync();
    }
}
