namespace NLUTakehome.Ingestion.Services;

using Npgsql;
using NLUTakehome.Ingestion.Parsers;
using NLUTakehome.Ingestion.Repositories;

public class ScofflawsIngestionService(string connectionString) : IIngestionService
{
    private const int BATCH_SIZE = 1000;

    public async Task IngestAsync(string path)
    {
        Console.WriteLine($"Starting ingestion of scofflaw data from {path}...");

        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var repo = new ScofflawsRepository(connection);
            var batch = new List<string>(BATCH_SIZE);

            foreach (var address in ScofflawParser.Parse(path))
            {
                batch.Add(address);

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
            Console.WriteLine($"File error during scofflaws ingestion: {ex.Message}");
            throw;
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"Database error during scofflaws ingestion: {ex.Message}");
            throw;
        }

        Console.WriteLine("Finished ingesting scofflaw data.");
    }
}
