namespace NLUTakehome.Endpoints;

using NLUTakehome.Models.Responses;
using NLUTakehome.Repositories;

public static class PropertyEndpoint
{
    public static void MapPropertyEndpoint(this WebApplication app)
    {
        app.MapGet("/property/{address}", async (string address, IPropertyRepository repo) =>
        {
            var normalizedAddress = address.Trim().ToLower();

            try
            {
                var (violations, isScofflaw) = await repo.GetViolationsAsync(normalizedAddress);

                if (violations.Count == 0) return Results.NotFound("No violations found for the given address.");

                return Results.Ok(new PropertyResponse(
                    violations[0].Date,
                    violations.Count,
                    violations,
                    isScofflaw
                ));
            }
            catch
            {
                return Results.Problem("A database error occurred while retrieving property data.");
            }
        });
    }
}
