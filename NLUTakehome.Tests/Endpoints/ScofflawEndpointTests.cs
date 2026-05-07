namespace NLUTakehome.Tests.Endpoints;

using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NLUTakehome.Repositories;

public class ScofflawEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ScofflawEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient(IScofflawRepository repo) =>
        _factory.WithWebHostBuilder(b =>
        {
            b.ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        "Host=localhost;Database=test;Username=test;Password=test"
                }));
            b.ConfigureTestServices(s =>
                s.AddScoped<IScofflawRepository>(_ => repo));
        }).CreateClient();

    [Fact]
    public async Task GetScofflaws_MissingSince_Returns400()
    {
        var client = CreateClient(new StubScofflawRepository([]));

        var response = await client.GetAsync("/property/scofflaws/violations");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetScofflaws_InvalidSinceFormat_Returns400()
    {
        var client = CreateClient(new StubScofflawRepository([]));

        var response = await client.GetAsync("/property/scofflaws/violations?since=08-14-2025");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetScofflaws_ValidSince_Returns200()
    {
        // Real addresses from Building_Code_Scofflaw_List_20250807.csv
        var client = CreateClient(new StubScofflawRepository(["500 e 88th st", "4822 w melrose st"]));

        var response = await client.GetAsync("/property/scofflaws/violations?since=2025-08-14");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetScofflaws_RepoThrows_Returns500()
    {
        var client = CreateClient(new ThrowingScofflawRepository());

        var response = await client.GetAsync("/property/scofflaws/violations?since=2025-08-14");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    private class StubScofflawRepository(List<string> addresses) : IScofflawRepository
    {
        public Task<List<string>> GetAddressesWithViolationsSinceAsync(DateOnly sinceDate) =>
            Task.FromResult(addresses);
    }

    private class ThrowingScofflawRepository : IScofflawRepository
    {
        public Task<List<string>> GetAddressesWithViolationsSinceAsync(DateOnly sinceDate) =>
            throw new Exception("DB failure");
    }
}
