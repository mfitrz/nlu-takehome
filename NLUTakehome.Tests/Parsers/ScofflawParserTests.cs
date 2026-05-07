namespace NLUTakehome.Tests.Parsers;

using NLUTakehome.Ingestion.Parsers;

public class ScofflawParserTests : IDisposable
{
    private readonly string _tempFile = Path.GetTempFileName();

    public void Dispose() => File.Delete(_tempFile);

    private void WriteCsv(string content) => File.WriteAllText(_tempFile, content);

    [Fact]
    public void Parse_ValidAddress_ReturnsLowercased()
    {
        // Real value from Building_Code_Scofflaw_List_20250807.csv
        WriteCsv("ADDRESS\n500 E 88TH ST\n");

        var results = ScofflawParser.Parse(_tempFile).ToList();

        Assert.Single(results);
        Assert.Equal("500 e 88th st", results[0]);
    }

    [Fact]
    public void Parse_EmptyAddress_SkipsRecord()
    {
        WriteCsv("ADDRESS\n   \n");

        var results = ScofflawParser.Parse(_tempFile).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Parse_MultipleAddresses_ReturnsAll()
    {
        // Real values from Building_Code_Scofflaw_List_20250807.csv
        WriteCsv("ADDRESS\n4822 W MELROSE ST\n3793 S ARCHER AVE\n");

        var results = ScofflawParser.Parse(_tempFile).ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains("4822 w melrose st", results);
        Assert.Contains("3793 s archer ave", results);
    }
}
