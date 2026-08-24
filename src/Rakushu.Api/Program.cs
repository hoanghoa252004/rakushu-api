using Rakushu.Api.Extensions;
using Rakushu.Api.Middleware;
using Rakushu.Application.Extensions;
using Rakushu.Infrastructure.Extensions;
using Rakushu.Persistence.Data;
using Rakushu.Persistence.DbContext;
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

/// SEED DATABASE IN DEVELOPMENT:
if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var dbContext = scope.ServiceProvider.GetRequiredService<RakushuDbContext>();
	try
	{
		await DbInitializer.InitializeAsync(dbContext);
	}
	catch (Exception ex)
	{
		app.Logger.LogError(ex, "An error occurred while initializing/seeding the database.");
	}
}

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
