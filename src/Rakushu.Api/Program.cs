using Rakushu.Api.Extensions;
using Rakushu.Api.Middleware;
using Rakushu.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

/// CONFIG SOFTWARE LAYERS:
builder.Services.AddRakushuApi();
builder.Services.AddRakushuApplication();

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

/// MAP ENDPOINTS:
app.MapEndpoints();
app.MapGet("/", () => "Welcome to Rakushu Api").ExcludeFromDescription();

app.Run();
