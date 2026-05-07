namespace NLUTakehome.Tests.Endpoints;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NLUTakehome.Models.Requests;
using NLUTakehome.Repositories;

public class CommentsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CommentsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient(ICommentsRepository repo) =>
        _factory.WithWebHostBuilder(b =>
        {
            b.ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        "Host=localhost;Database=test;Username=test;Password=test"
                }));
            b.ConfigureTestServices(s =>
                s.AddScoped(_ => repo));
        }).CreateClient();

    [Fact]
    public async Task PostComment_EmptyAuthor_Returns400()
    {
        var client = CreateClient(new StubCommentsRepository(propertyExists: true));
        var body = new CommentRequest("", "Looks good.");

        var response = await client.PostAsJsonAsync("/property/4822 w melrose st/comments", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostComment_PropertyNotFound_Returns404()
    {
        var client = CreateClient(new StubCommentsRepository(propertyExists: false));
        var body = new CommentRequest("Alice", "Looks good.");

        var response = await client.PostAsJsonAsync("/property/500 e 88th st/comments", body);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostComment_ValidRequest_Returns201()
    {
        var client = CreateClient(new StubCommentsRepository(propertyExists: true));
        var body = new CommentRequest("Alice", "Looks good.");

        var response = await client.PostAsJsonAsync("/property/4822 w melrose st/comments", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostComment_RepoThrows_Returns500()
    {
        var client = CreateClient(new ThrowingCommentsRepository());
        var body = new CommentRequest("Alice", "Looks good.");

        var response = await client.PostAsJsonAsync("/property/3793 s archer ave/comments", body);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    private class StubCommentsRepository(bool propertyExists) : ICommentsRepository
    {
        public Task<bool> PropertyExistsAsync(string address) => Task.FromResult(propertyExists);
        public Task InsertCommentAsync(string address, string author, string comment) => Task.CompletedTask;
    }

    private class ThrowingCommentsRepository : ICommentsRepository
    {
        public Task<bool> PropertyExistsAsync(string address) => throw new Exception("DB failure");
        public Task InsertCommentAsync(string address, string author, string comment) => Task.CompletedTask;
    }
}
