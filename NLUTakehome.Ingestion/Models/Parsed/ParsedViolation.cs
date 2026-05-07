namespace NLUTakehome.Ingestion.Models.Parsed;

public record ParsedViolation(
    int Id,
    string Address,
    DateOnly? ViolationDate,
    string? Code,
    string? Status,
    string? Desc,
    string? InspectorComments
);
