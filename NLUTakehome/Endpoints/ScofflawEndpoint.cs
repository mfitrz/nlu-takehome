namespace NLUTakehome.Endpoints;

using NLUTakehome.Models.Responses;
using NLUTakehome.Repositories;
using NLUTakehome.Validators;

public static class ScofflawEndpoint
{
    public static void MapScofflawEndpoint(this WebApplication app)
    {
        app.MapGet("/property/scofflaws/violations", async (string? since, IScofflawRepository repo) =>
        {
            var error = ScofflawValidator.Validate(since, out var sinceDate);
            if (error != null) return error;

            try
            {
                var addresses = await repo.GetAddressesWithViolationsSinceAsync(sinceDate);
                return Results.Ok(new ScofflawResponse(addresses));
            }
            catch
            {
                return Results.Problem("A database error occurred while retrieving scofflaw data.");
            }
        });
    }
}
