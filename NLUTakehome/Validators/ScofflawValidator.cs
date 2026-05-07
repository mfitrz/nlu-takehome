namespace NLUTakehome.Validators;

public static class ScofflawValidator
{
    public static IResult? Validate(string? since, out DateOnly sinceDate)
    {
        if (string.IsNullOrEmpty(since) || !DateOnly.TryParseExact(since, "yyyy-MM-dd", out sinceDate))
        {
            sinceDate = default;
            return Results.BadRequest("Query parameter 'since' must be a valid date in yyyy-MM-dd format.");
        }

        return null;
    }
}
