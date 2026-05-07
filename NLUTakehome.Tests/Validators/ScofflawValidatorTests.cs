namespace NLUTakehome.Tests.Validators;

using NLUTakehome.Validators;

public class ScofflawValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-date")]
    [InlineData("05/07/2026")]
    public void Validate_InvalidSince_ReturnsBadRequest(string? since)
    {
        var result = ScofflawValidator.Validate(since, out var sinceDate);

        Assert.NotNull(result);
        Assert.Equal(default, sinceDate);
    }

    [Fact]
    public void Validate_ValidSince_ReturnsNullAndParsedDate()
    {
        var result = ScofflawValidator.Validate("2026-01-15", out var sinceDate);

        Assert.Null(result);
        Assert.Equal(new DateOnly(2026, 1, 15), sinceDate);
    }

    [Fact]
    public void Validate_ValidSince_DateBoundary()
    {
        var result = ScofflawValidator.Validate("2000-01-01", out var sinceDate);

        Assert.Null(result);
        Assert.Equal(new DateOnly(2000, 1, 1), sinceDate);
    }
}
