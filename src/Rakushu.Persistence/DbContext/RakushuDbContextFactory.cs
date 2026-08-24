using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Rakushu.Persistence.DbContext;

public class RakushuDbContextFactory : IDesignTimeDbContextFactory<RakushuDbContext>
{
	public RakushuDbContext CreateDbContext(string[] args)
	{
		// Try to read appsettings.json from Rakushu.Api if available
		var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Rakushu.Api");
		if (!Directory.Exists(basePath))
		{
			basePath = Path.Combine(Directory.GetCurrentDirectory(), "src/Rakushu.Api");
		}
		if (!Directory.Exists(basePath))
		{
			basePath = Directory.GetCurrentDirectory();
		}

		var configuration = new ConfigurationBuilder()
			.SetBasePath(basePath)
			.AddJsonFile("appsettings.json", optional: true)
			.AddJsonFile("appsettings.Development.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

		var connectionString = configuration.GetConnectionString("DefaultConnection")
			?? configuration.GetConnectionString("Database")
			?? "Host=localhost;Port=5432;Database=rakushu_db;Username=postgres;Password=postgres";

		var optionsBuilder = new DbContextOptionsBuilder<RakushuDbContext>();
		optionsBuilder.UseNpgsql(connectionString)
			.UseSnakeCaseNamingConvention();

		return new RakushuDbContext(optionsBuilder.Options);
	}
}
