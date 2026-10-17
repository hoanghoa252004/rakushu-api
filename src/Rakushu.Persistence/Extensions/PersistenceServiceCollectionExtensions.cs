using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Domain.Common.Contract;
using Rakushu.Persistence.Connection;
using Rakushu.Persistence.Queries;
using Rakushu.Persistence.Repositories;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.NotReviewedYet.OovCandidate;
using Rakushu.Domain.Entities.NotReviewedYet.DictionaryEntry;
using Rakushu.Domain.Entities.Subscriptions.Plan;
using Rakushu.Domain.Entities.Subscriptions.Subscription;
using Rakushu.Domain.Entities.Subscriptions.Feature;
using Rakushu.Domain.Entities.Subscriptions.Payment;
using Rakushu.Domain.Entities.Subscriptions.Transaction;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;
using Rakushu.Domain.Entities.Srs;

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
		services.AddScoped<IOovCandidateRepository, OovCandidateRepository>();
		services.AddScoped<IDictionaryEntryRepository, DictionaryEntryRepository>();
		services.AddScoped<IContentCategoryRepository, ContentCategoryRepository>();
		services.AddScoped<IVideoRepository, VideoRepository>();
		services.AddScoped<IJapanesePartOfSpeechRepository, JapanesePartOfSpeechRepository>();
		services.AddScoped<IDependencyRelationshipRepository, DependencyRelationshipRepository>();
		services.AddScoped<IProficiencyLevelRepository, ProficiencyLevelRepository>();
		services.AddScoped<ISeriesRepository, SeriesRepository>();
		services.AddScoped<ISrsNoteRepository, SrsNoteRepository>();
		services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
		services.AddScoped<ISrsCardRepository, SrsCardRepository>();

		DefaultTypeMap.MatchNamesWithUnderscores = true;
		services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();

		services.AddScoped<IOovCandidateQuery, OovCandidateQuery>();
		return services;
	}
}

