using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Application.Usecases.Embedding;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Token;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using Rakushu.Domain.Entities.Subscriptions.Feature;
using Rakushu.Domain.Entities.Subscriptions.Payment;
using Rakushu.Domain.Entities.Subscriptions.Plan;
using Rakushu.Domain.Entities.Subscriptions.Subscription;
using Rakushu.Domain.Entities.Subscriptions.Transaction;
using Rakushu.Persistence.Connection;
using Rakushu.Persistence.Repositories;

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
				npgsqlOptions.UseVector();
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
		services.AddScoped<IContentCategoryRepository, ContentCategoryRepository>();
		services.AddScoped<IVideoRepository, VideoRepository>();
		services.AddScoped<IJapanesePartOfSpeechRepository, JapanesePartOfSpeechRepository>();
		services.AddScoped<IDependencyRelationshipRepository, DependencyRelationshipRepository>();
		services.AddScoped<IProficiencyLevelRepository, ProficiencyLevelRepository>();
		services.AddScoped<ISeriesRepository, SeriesRepository>();
		services.AddScoped<IKnowledgeMeaningRepository, KnowledgeMeaningRepository>();
		//services.AddScoped<IKnowledgeEmbeddingRepository, KnowledgeEmbeddingRepository>();
		services.AddScoped<IKnowledgeCollectionRepository, KnowledgeCollectionRepository>();
		services.AddScoped<ITokenRepository, TokenRepository>();
		services.AddScoped<IKnowledgeUnitRepository, KnowledgeUnitRepository>();
		services.AddScoped<Rakushu.Domain.Entities.Material.LearningUnit.ILearningUnitRepository, LearningUnitRepository>();
		services.AddScoped<IContentKnowledgeCollectionRepository, ContentKnowledgeCollectionRepository>();
		services.AddScoped<ITranscriptRepository, TranscriptRepository>();
		services.AddScoped<ISpeakerRepository, SpeakerRepository>();


		DefaultTypeMap.MatchNamesWithUnderscores = true;
		services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();
		return services;
	}
}

