using Rakushu.Api.Extensions;
using Rakushu.Api.Middleware;
using Rakushu.Application.Extensions;
using Rakushu.Infrastructure.Extensions;
using Rakushu.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

/// CONFIG SOFTWARE LAYERS:
builder.Services.AddRakushuPersistence(builder.Configuration);
builder.Services.AddRakushuInfrastructure(builder.Configuration);
builder.Services.AddRakushuApplication();
builder.Services.AddRakushuApi();

/// CONFIG MIDDLEWARES:
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

/// CONFIG SWAGGER DOCS:
builder.Services.AddSwaggerDocs();

var app = builder.Build();

/// USE GLOBAL EXCEPTION MIDDLEWARE:
app.UseExceptionHandler();

/// USE SWAGGER DOCS:
app.UseSwaggerDocs();

/// AUTHENTICATION & AUTHORIZATION:
app.UseAuthentication();
app.UseAuthorization();

/// MAP ENDPOINTS:
app.MapEndpoints();
app.MapGet("/", () => "Welcome to Rakushu Api").ExcludeFromDescription();

app.Run();
