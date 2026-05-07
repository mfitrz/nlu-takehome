namespace NLUTakehome.Endpoints;

using NLUTakehome.Models.Requests;
using NLUTakehome.Models.Responses;
using NLUTakehome.Repositories;
using NLUTakehome.Validators;

public static class CommentsEndpoint
{
    public static void MapCommentsEndpoint(this WebApplication app)
    {
        app.MapPost("/property/{address}/comments", async (string address, CommentRequest body, ICommentsRepository repo) =>
        {
            var error = CommentValidator.Validate(body);
            if (error != null) return error;

            var normalizedAddress = address.Trim().ToLower();
            var cleanAuthor = body.Author.Trim();
            var cleanComment = body.Comment.Trim();

            try
            {
                if (!await repo.PropertyExistsAsync(normalizedAddress))
                    return Results.NotFound("Comments must be provided for an existing property.");

                await repo.InsertCommentAsync(normalizedAddress, cleanAuthor, cleanComment);

                return Results.Created($"/property/{address}/comments", new CommentResponse("Comment added successfully."));
            }
            catch
            {
                return Results.Problem("An unexpected database error occurred while saving the comment.");
            }
        });
    }
}
