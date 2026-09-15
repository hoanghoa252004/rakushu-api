using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Persistence.Queries;
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

		// UNIT OF WORK
		services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RakushuDbContext>());

		// REPOSITORIES
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IRoleRepository, RoleRepository>();
			services.AddScoped<IPlanRepository, PlanRepository>();

			// QUERIES
				services.AddScoped<IUserQuery, UserQuery>();
				services.AddScoped<IRoleQuery, RoleQuery>();
				services.AddScoped<IPlanQuery, PlanQuery>();
				return services;
	}
}
