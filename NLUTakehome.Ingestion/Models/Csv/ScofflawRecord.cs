namespace NLUTakehome.Ingestion.Models.Csv;

using CsvHelper.Configuration.Attributes;

public class ScofflawRecord
{
    [Name("ADDRESS")]
    public required string Address { get; set; }
}
