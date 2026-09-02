using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Repositories;
using Rakushu.Persistence.Repositories;

namespace Rakushu.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRakushuPersistence(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		// CONFIGURE DBCONTEXT EFCORE ---> POSTGRESQL
		services.AddDbContext<RakushuDbContext>(options =>
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

			options.UseNpgsql(connectionString, npgsqlOptions =>
			{
				npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
				npgsqlOptions.EnableRetryOnFailure(
					maxRetryCount: 3,
					maxRetryDelay: TimeSpan.FromSeconds(30),
					errorCodesToAdd: null);
			})
			.UseSnakeCaseNamingConvention();
		});

		// REGISTER REPOSITORIES AND UNIT OF WORK
		services.AddScoped<IUnitOfWork, RakushuDbContext>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IRoleRepository, RoleRepository>();

		return services;
	}
}
