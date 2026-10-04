using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.Transaction;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Persistence.Connection;
using Rakushu.Persistence.Queries;
using Rakushu.Persistence.Repositories;
using Rakushu.Persistence.Queries.User;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;
using Rakushu.Domain.Entities.Linguistic.DependencyRelationship;
using Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm;
using Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech;
using Rakushu.Domain.Entities.SupportedLanguage;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

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
		services.AddScoped<Domain.Entities.OovCandidate.IOovCandidateRepository, OovCandidateRepository>();
		services.AddScoped<Domain.Entities.DictionaryEntry.IDictionaryEntryRepository, DictionaryEntryRepository>();
		services.AddScoped<IContentCategoryRepository, ContentCategoryRepository>();
		services.AddScoped<IProficiencyFrameworkRepository, ProficiencyFrameworkRepository>();
		services.AddScoped<IVideoRepository, VideoRepository>();
		services.AddScoped<IJapaneseConjugationFormRepository, JapaneseConjugationFormRepository>();
		services.AddScoped<IJapanesePartOfSpeechRepository, JapanesePartOfSpeechRepository>();
		services.AddScoped<IUniversalPartOfSpeechRepository, UniversalPartOfSpeechRepository>();
		services.AddScoped<IDependencyRelationshipRepository, DependencyRelationshipRepository>();
		services.AddScoped<ISupportedLanguageRepository, SupportedLanguageRepository>();
		services.AddScoped<IProficiencyLevelRepository, ProficiencyLevelRepository>();

		DefaultTypeMap.MatchNamesWithUnderscores = true;
		services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();

		services.AddScoped<IUserQuery, UserQuery>();
		services.AddScoped<IRoleQuery, RoleQuery>();
		services.AddScoped<IPlanQuery, PlanQuery>();
		services.AddScoped<IFeatureQuery, FeatureQuery>();
		services.AddScoped<IEntitlementQuery, EntitlementQuery>();
		services.AddScoped<IPaymentQuery, PaymentQuery>();
		services.AddScoped<ITransactionQuery, TransactionQuery>();
		services.AddScoped<IOovCandidateQuery, OovCandidateQuery>();

		return services;
	}
}
