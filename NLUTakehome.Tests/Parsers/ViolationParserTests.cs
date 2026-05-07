namespace NLUTakehome.Tests.Parsers;

using NLUTakehome.Ingestion.Parsers;

public class ViolationParserTests : IDisposable
{
    private readonly string _tempFile = Path.GetTempFileName();

    public void Dispose() => File.Delete(_tempFile);

    private void WriteCsv(string content) => File.WriteAllText(_tempFile, content);

    private const string Headers =
        "ID,ADDRESS,VIOLATION DATE,VIOLATION CODE,VIOLATION STATUS,VIOLATION DESCRIPTION,VIOLATION INSPECTOR COMMENTS";

    [Fact]
    public void Parse_ValidRecord_ReturnsLowercasedAddress()
    {
        // Real values from Building_Violations_20250815.csv
        WriteCsv(Headers + "\n" +
                 "7373645,7120 S ROCKWELL ST,08/14/2025,CN104035,OPEN,MAINTAIN WINDOW,BASEMENT - WINDOWS PANES BROKEN\n");

        var results = ViolationParser.Parse(_tempFile).ToList();

        Assert.Single(results);
        Assert.Equal("7120 s rockwell st", results[0].Address);
        Assert.Equal(new DateOnly(2025, 8, 14), results[0].ViolationDate);
        Assert.Equal("CN104035", results[0].Code);
    }

    [Fact]
    public void Parse_EmptyAddress_SkipsRecord()
    {
        WriteCsv(Headers + "\n" +
                 "7373792,   ,08/14/2025,CN104035,OPEN,MAINTAIN WINDOW,WEST ELEVATION - BROKEN WINDOW\n");

        var results = ViolationParser.Parse(_tempFile).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Parse_InvalidDateFormat_SetsViolationDateNull()
    {
        // Real address, bad date format (yyyy-MM-dd instead of MM/dd/yyyy)
        WriteCsv(Headers + "\n" +
                 "7373987,5126 S MICHIGAN AVE,2025-08-14,CN070014,OPEN,REPAIR EXTERIOR STAIR,HANDRAIL MISSING\n");

        var results = ViolationParser.Parse(_tempFile).ToList();

        Assert.Single(results);
        Assert.Null(results[0].ViolationDate);
    }
}
