namespace NLUTakehome.Ingestion.Parsers;

using System.Globalization;
using CsvHelper;
using NLUTakehome.Ingestion.Models.Csv;

public static class ScofflawParser
{
    public static IEnumerable<string> Parse(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        foreach (var record in csv.GetRecords<ScofflawRecord>())
        {
            var address = record.Address?.Trim().ToLower();
            if (string.IsNullOrEmpty(address))
            {
                Console.WriteLine("Skipping scofflaw record with empty address.");
                continue;
            }

            yield return address;
        }
    }
}
