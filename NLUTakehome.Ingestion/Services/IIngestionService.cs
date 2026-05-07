namespace NLUTakehome.Ingestion.Services;

public interface IIngestionService
{
    Task IngestAsync(string path);
}
