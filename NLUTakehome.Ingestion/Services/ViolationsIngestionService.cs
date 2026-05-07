namespace NLUTakehome.Ingestion.Services;

using Npgsql;
using NLUTakehome.Ingestion.Models.Parsed;
using NLUTakehome.Ingestion.Parsers;
using NLUTakehome.Ingestion.Repositories;

public class ViolationsIngestionService(string connectionString) : IIngestionService
{
    private const int BATCH_SIZE = 1000;

    public async Task IngestAsync(string path)
    {
        Console.WriteLine($"Starting ingestion of violation data from {path}...");

        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var repo = new ViolationsRepository(connection);
            var batch = new List<ParsedViolation>(BATCH_SIZE);

            foreach (var record in ViolationParser.Parse(path))
            {
                batch.Add(record);

                if (batch.Count == BATCH_SIZE)
                {
                    await repo.InsertBatchAsync(batch);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
                await repo.InsertBatchAsync(batch);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"File error during violations ingestion: {ex.Message}");
            throw;
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"Database error during violations ingestion: {ex.Message}");
            throw;
        }

        Console.WriteLine("Finished ingesting violation data.");
    }
}
