namespace NLUTakehome.Ingestion.Parsers;

using System.Globalization;
using CsvHelper;
using NLUTakehome.Ingestion.Models.Csv;
using NLUTakehome.Ingestion.Models.Parsed;

public static class ViolationParser
{
    public static IEnumerable<ParsedViolation> Parse(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        foreach (var record in csv.GetRecords<ViolationRecord>())
        {
            var address = record.Address?.Trim();
            if (string.IsNullOrEmpty(address))
            {
                Console.WriteLine("Skipping violation record with empty address.");
                continue;
            }

            DateOnly? violationDate = null;
            if (!string.IsNullOrWhiteSpace(record.ViolationDate))
            {
                if (DateOnly.TryParseExact(record.ViolationDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                    violationDate = parsed;
                else
                    Console.WriteLine($"Skipping unrecognized date format for record {record.ID}: '{record.ViolationDate}'");
            }

            yield return new ParsedViolation(
                record.ID,
                address.ToLower(),
                violationDate,
                record.ViolationCode,
                record.ViolationStatus,
                record.ViolationDesc,
                record.ViolationInspcComms
            );
        }
    }
}
