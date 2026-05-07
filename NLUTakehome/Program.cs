using DotNetEnv;
using NLUTakehome.Endpoints;
using NLUTakehome.Repositories;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddHttpLogging(o => { });
builder.Services.AddNpgsqlDataSource(connectionString);
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IScofflawRepository, ScofflawRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseHttpLogging();

app.MapPropertyEndpoint();
app.MapCommentsEndpoint();
app.MapScofflawEndpoint();

app.Run();
