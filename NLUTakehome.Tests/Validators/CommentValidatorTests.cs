namespace NLUTakehome.Tests.Validators;

using NLUTakehome.Models.Requests;
using NLUTakehome.Validators;

public class CommentValidatorTests
{
    [Fact]
    public void Validate_NullBody_ReturnsBadRequest()
    {
        var result = CommentValidator.Validate(null);

        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("", "Some comment")]
    [InlineData("   ", "Some comment")]
    [InlineData("Author", "")]
    [InlineData("Author", "   ")]
    public void Validate_MissingAuthorOrComment_ReturnsBadRequest(string author, string comment)
    {
        var result = CommentValidator.Validate(new CommentRequest(author, comment));

        Assert.NotNull(result);
    }

    [Fact]
    public void Validate_ValidRequest_ReturnsNull()
    {
        var result = CommentValidator.Validate(new CommentRequest("Alice", "Great building."));

        Assert.Null(result);
    }
}
