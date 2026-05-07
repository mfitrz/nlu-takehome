namespace NLUTakehome.Models.Responses;

using NLUTakehome.Models.Db;

public record PropertyResponse(
    DateOnly LastViolationDate,
    int TotalViolations,
    List<ViolationRow> Violations,
    bool IsScofflaw
);
