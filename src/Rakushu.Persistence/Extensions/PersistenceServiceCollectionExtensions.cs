using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.Transaction;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;
using Rakushu.Persistence.Connection;
using Rakushu.Persistence.Queries;
using Rakushu.Persistence.Repositories;
using Rakushu.Domain.Entities.ProficiencyLevel;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;
using Rakushu.Application.Abstractions.Persistence.Queries;

namespace Rakushu.Persistence.Extensions;

public static class PersistenceServiceCollectionExtensions
{
	public static IServiceCollection AddRakushuPersistence(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContext<RakushuDbContext>(options =>
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

			options.UseNpgsql(connectionString, npgsqlOptions =>
			{
				npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
				npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
			}).UseSnakeCaseNamingConvention();
		});

		services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RakushuDbContext>());

		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IRoleRepository, RoleRepository>();
		services.AddScoped<IPlanRepository, PlanRepository>();
		services.AddScoped<IFeatureRepository, FeatureRepository>();
		services.AddScoped<IPaymentRepository, PaymentRepository>();
		services.AddScoped<ITransactionRepository, TransactionRepository>();
		services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
		services.AddScoped<Domain.Entities.OovCandidate.IOovCandidateRepository, OovCandidateRepository>();
		services.AddScoped<Domain.Entities.DictionaryEntry.IDictionaryEntryRepository, DictionaryEntryRepository>();
		services.AddScoped<IContentCategoryRepository, ContentCategoryRepository>();
		services.AddScoped<IVideoRepository, VideoRepository>();
		services.AddScoped<IJapanesePartOfSpeechRepository, JapanesePartOfSpeechRepository>();
		services.AddScoped<IUniversalPartOfSpeechRepository, UniversalPartOfSpeechRepository>();
		services.AddScoped<IDependencyRelationshipRepository, DependencyRelationshipRepository>();
		services.AddScoped<IProficiencyLevelRepository, ProficiencyLevelRepository>();

		DefaultTypeMap.MatchNamesWithUnderscores = true;
		services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();

		services.AddScoped<IOovCandidateQuery, OovCandidateQuery>();
		return services;
	}
}

