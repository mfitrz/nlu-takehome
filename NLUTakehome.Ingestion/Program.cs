using DotNetEnv;
using NLUTakehome.Ingestion.Services;

Env.TraversePath().Load();

var supabaseConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'ConnectionStrings__DefaultConnection' is not set in .env.");

var scofflawFilePath = "datasets/Building_Code_Scofflaw_List_20250807.csv";
var violationFilePath = "datasets/Building_Violations_20250815.csv";

try
{
    await Task.WhenAll(
        new ScofflawsIngestionService(supabaseConnection).IngestAsync(scofflawFilePath),
        new ViolationsIngestionService(supabaseConnection).IngestAsync(violationFilePath)
    );

    Console.WriteLine("Ingestion complete!");
}
catch (Exception ex)
{
    Console.WriteLine($"Ingestion failed: {ex.Message}");
    Environment.Exit(1);
}