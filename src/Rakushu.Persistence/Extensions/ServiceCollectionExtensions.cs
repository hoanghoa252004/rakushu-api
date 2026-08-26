using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Repositories;
using Rakushu.Persistence.DbContext;
using Rakushu.Persistence.Repositories;
using Rakushu.Persistence.UnitOfWork;

namespace Rakushu.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRakushuPersistence(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection")
			?? configuration.GetConnectionString("Database")
			?? "Host=localhost;Port=5432;Database=rakushu_db;Username=postgres;Password=postgres";

		services.AddDbContext<RakushuDbContext>(options =>
		{
			options.UseNpgsql(connectionString)
				.UseSnakeCaseNamingConvention();
		});

		services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IRoleRepository, RoleRepository>();

		return services;
	}
}
