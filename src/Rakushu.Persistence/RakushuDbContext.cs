using MediatR;
using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Events.DomainEvent;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternElement;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternRelation;
using Rakushu.Domain.Entities.Knowledge.LinguisticKnowledge;
using Rakushu.Domain.Entities.LearningUnit;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.BunsetsuDependencyRelationship;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.Transaction;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Plan.PlanEntitlement;
using Rakushu.Domain.Entities.ProficiencyFramework;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.Series;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.EmailVerificationToken;
using Rakushu.Domain.Entities.User.Profile;
using Rakushu.Domain.Entities.User.Profile.Interest;
using Rakushu.Domain.Entities.User.RefreshToken;
using Rakushu.Domain.Entities.User.Subscription;
using Rakushu.Domain.Entities.User.Subscription.SubscriptionUsage;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.MediaAsset;
using Rakushu.Domain.Entities.Video.Subtitle;
using Rakushu.Domain.Entities.Video.Subtitle.SubtitleSegment;
using Rakushu.Domain.Entities.Video.Transcript;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;
using Rakushu.Domain.SupportedLanguage;

namespace Rakushu.Persistence;

public class RakushuDbContext : DbContext, IUnitOfWork
{
	private readonly IPublisher _publisher;
	public RakushuDbContext( 
		DbContextOptions<RakushuDbContext> options, 
		IPublisher publisher) : base(options)
	{
		_publisher = publisher;
	}

	// User Management
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<User> Users => Set<User>();
	public DbSet<Profile> Profiles => Set<Profile>();
	public DbSet<Interest> Interests => Set<Interest>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<EmailVerificationToken> EmailVerificationToken => Set<EmailVerificationToken>();

	// Plans & Features
	public DbSet<Plan> Plans => Set<Plan>();
	public DbSet<Feature> Features => Set<Feature>();
	public DbSet<PlanEntitlement> PlanEntitlements => Set<PlanEntitlement>();

	// Subscriptions & Payments
	public DbSet<Subscription> Subscriptions => Set<Subscription>();
	public DbSet<SubscriptionUsage> SubscriptionUsages => Set<SubscriptionUsage>();
	public DbSet<Payment> Payments => Set<Payment>();
	public DbSet<Transaction> Transactions => Set<Transaction>();

	// Proficiency Framework
	public DbSet<ProficiencyFramework> ProficiencyFrameworks => Set<ProficiencyFramework>();
	public DbSet<ProficiencyLevel> ProficiencyLevels => Set<ProficiencyLevel>();
	public DbSet<ProficiencyEquivalence> ProficiencyEquivalences => Set<ProficiencyEquivalence>();

	// Video & Learning
	public DbSet<Video> Videos => Set<Video>();
	public DbSet<Series> Series => Set<Series>();
	public DbSet<Transcript> Transcripts => Set<Transcript>();
	public DbSet<TranscriptSegment> TranscriptSegments => Set<TranscriptSegment>();
	public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
	public DbSet<Subtitle> Subtitles => Set<Subtitle>();
	public DbSet<SubtitleSegment> SubtitleItems => Set<SubtitleSegment>();

	// Learning Units
	public DbSet<LearningUnit> LearningUnits => Set<LearningUnit>();
	public DbSet<Bunsetsu> Bunsetsu => Set<Bunsetsu>();
	public DbSet<Token> Tokens => Set<Token>();
	public DbSet<BunsetsuDependencyRelationship> BunsetsuDependencyRelationships => Set<BunsetsuDependencyRelationship>();

	// Content & Categories
	public DbSet<ContentCategory> ContentCategories => Set<ContentCategory>();

	// Knowledge
	public DbSet<LinguisticKnowledge> LinguisticKnowledges => Set<LinguisticKnowledge>();
	public DbSet<KnowledgePattern> KnowledgePatterns => Set<KnowledgePattern>();
	public DbSet<KnowledgePatternElement> KnowledgePatternElements => Set<KnowledgePatternElement>();
	public DbSet<KnowledgePatternRelation> KnowledgePatternRelations => Set<KnowledgePatternRelation>();
	public DbSet<KnowledgeMeaning> KnowledgeMeanings => Set<KnowledgeMeaning>();

	// Linguistic Metadata
	public DbSet<JapanesePartOfSpeech> JapanesePartOfSpeeches => Set<JapanesePartOfSpeech>();
	public DbSet<UniversalPartOfSpeech> UniversalPartOfSpeeches => Set<UniversalPartOfSpeech>();
	public DbSet<JapaneseConjugationForm> JapaneseConjugationForms => Set<JapaneseConjugationForm>();
	public DbSet<DependencyRelationship> DependencyRelationships => Set<DependencyRelationship>();

	// Supported Languages
	public DbSet<SupportedLanguage> SupportedLanguages => Set<SupportedLanguage>();


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(RakushuDbContext).Assembly);
	}

	public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
	{
		var strategy = Database.CreateExecutionStrategy();
		return await strategy.ExecuteAsync(async () =>
		{
			await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
			{
				var response = await action();
				if (response is Result result && result.IsFailure)
				{
					await transaction.RollbackAsync(cancellationToken);
					return response;
				}
				do
				{
					var domainEvents = GetDomainEvents();
					if (domainEvents.Any())
					{
						await DispatchDomainEventsAsync(domainEvents, cancellationToken);
					}
				} while (CheckDomainEventRemain());


				await SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);

				return response;
			}
		});
	}

	private List<IDomainEvent> GetDomainEvents()
	{
		var domainEventEntities = ChangeTracker.Entries()
			.Select(e => e.Entity)
			.OfType<IHasDomainEvents>()
			.Where(e => e.DomainEvents.Any())
			.ToList();

		var domainEvents = domainEventEntities
			.SelectMany(e => e.DomainEvents)
			.ToList();

		domainEventEntities.ForEach(e => e.ClearDomainEvents());

		return domainEvents;
	}

	private bool CheckDomainEventRemain()
	{
		var domainEventEntities = ChangeTracker.Entries()
			.Select(e => e.Entity)
			.OfType<IHasDomainEvents>()
			.Where(e => e.DomainEvents.Any())
			.ToList();

		var domainEvents = domainEventEntities
			.SelectMany(e => e.DomainEvents)
			.ToList();

		return domainEvents.Any();
	}

	private async Task DispatchDomainEventsAsync(List<IDomainEvent> domainEvents, CancellationToken cancellationToken)
	{
		foreach (var domainEvent in domainEvents)
		{
			await _publisher.Publish(domainEvent, cancellationToken);
		}
	}
}
