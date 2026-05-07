namespace NLUTakehome.Validators;

using NLUTakehome.Models.Requests;

public static class CommentValidator
{
    public static IResult? Validate(CommentRequest? body)
    {
        if (body == null)
            return Results.BadRequest("Request body is required.");

        if (string.IsNullOrWhiteSpace(body.Author) || string.IsNullOrWhiteSpace(body.Comment))
            return Results.BadRequest("Author and Comment cannot be empty.");

        return null;
    }
}
