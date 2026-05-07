namespace NLUTakehome.Models.Db;

public record ViolationRow(
    DateOnly Date,
    string? Code,
    string? Status,
    string? Description,
    string? InspectorComments
);
