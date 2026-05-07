namespace NLUTakehome.Tests.Endpoints;

using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NLUTakehome.Models.Db;
using NLUTakehome.Repositories;

public class PropertyEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PropertyEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient(IPropertyRepository repo) =>
        _factory.WithWebHostBuilder(b =>
        {
            b.ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        "Host=localhost;Database=test;Username=test;Password=test"
                }));
            b.ConfigureTestServices(s =>
                s.AddScoped<IPropertyRepository>(_ => repo));
        }).CreateClient();

    [Fact]
    public async Task GetProperty_NoViolationsFound_Returns404()
    {
        var client = CreateClient(new StubPropertyRepository([]));

        var response = await client.GetAsync("/property/500 e 88th st");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProperty_ViolationsFound_Returns200()
    {
        var violation = new ViolationRow(new DateOnly(2025, 8, 14), "CN104035", "OPEN", "MAINTAIN WINDOW", null);
        var client = CreateClient(new StubPropertyRepository([violation]));

        var response = await client.GetAsync("/property/7120 s rockwell st");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProperty_RepoThrows_Returns500()
    {
        var client = CreateClient(new ThrowingPropertyRepository());

        var response = await client.GetAsync("/property/3550 n lake shore dr");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    private class StubPropertyRepository(List<ViolationRow> violations) : IPropertyRepository
    {
        public Task<(List<ViolationRow> Violations, bool IsScofflaw)> GetViolationsAsync(string address) =>
            Task.FromResult((violations, false));
    }

    private class ThrowingPropertyRepository : IPropertyRepository
    {
        public Task<(List<ViolationRow> Violations, bool IsScofflaw)> GetViolationsAsync(string address) =>
            throw new Exception("DB failure");
    }
}
