namespace NLUTakehome.Ingestion.Models.Csv;

using CsvHelper.Configuration.Attributes;

public class ViolationRecord
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("ADDRESS")]
    public required string Address { get; set; }

    [Name("VIOLATION DATE")]
    public string? ViolationDate { get; set; }

    [Name("VIOLATION CODE")]
    public string? ViolationCode { get; set; }

    [Name("VIOLATION STATUS")]
    public string? ViolationStatus { get; set; }

    [Name("VIOLATION DESCRIPTION")]
    public string? ViolationDesc { get; set; }

    [Name("VIOLATION INSPECTOR COMMENTS")]
    public string? ViolationInspcComms { get; set; }
}
