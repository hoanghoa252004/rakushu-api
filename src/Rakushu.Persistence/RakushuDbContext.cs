using MediatR;
using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Events.DomainEvent;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Identity.User.EmailVerificationToken;
using Rakushu.Domain.Entities.Identity.User.Profile;
using Rakushu.Domain.Entities.Identity.User.Profile.Interest;
using Rakushu.Domain.Entities.Identity.User.RefreshToken;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Linguistic.DependencyRelationship;
using Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Bunsetsu.BunsetsuDependencyRelationship;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Subtitle;
using Rakushu.Domain.Entities.Material.Subtitle.SubtitleSegment;
using Rakushu.Domain.Entities.Material.Token;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;
using Rakushu.Domain.Entities.NotReviewedYet.CuratorReview;
using Rakushu.Domain.Entities.NotReviewedYet.DictionaryEntry;
using Rakushu.Domain.Entities.NotReviewedYet.OovCandidate;
using Rakushu.Domain.Entities.Subscriptions.Entitlement;
using Rakushu.Domain.Entities.Subscriptions.Feature;
using Rakushu.Domain.Entities.Subscriptions.Payment;
using Rakushu.Domain.Entities.Subscriptions.Plan;
using Rakushu.Domain.Entities.Subscriptions.Subscription;
using Rakushu.Domain.Entities.Subscriptions.Subscription.SubscriptionUsage;
using Rakushu.Domain.Entities.Subscriptions.Transaction;

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
	public DbSet<Entitlement> Entitlements => Set<Entitlement>();

	// Subscriptions & Payments
	public DbSet<Subscription> Subscriptions => Set<Subscription>();
	public DbSet<SubscriptionUsage> SubscriptionUsages => Set<SubscriptionUsage>();
	public DbSet<Payment> Payments => Set<Payment>();
	public DbSet<Transaction> Transactions => Set<Transaction>();
	public DbSet<OovCandidate> OovCandidates => Set<OovCandidate>();
	public DbSet<CuratorReview> CuratorReviews => Set<CuratorReview>();
	public DbSet<DictionaryEntry> DictionaryEntries => Set<DictionaryEntry>();

	// Proficiency Levels
	public DbSet<ProficiencyLevel> ProficiencyLevels => Set<ProficiencyLevel>();

	// Learning Contents
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
	public DbSet<ContentKnowledgeCollection> ContentProcessingPolicies => Set<ContentKnowledgeCollection>();

	// Knowledge
	public DbSet<KnowledgeUnit> LinguisticKnowledges => Set<KnowledgeUnit>();
	public DbSet<KnowledgeMeaning> KnowledgeMeanings => Set<KnowledgeMeaning>();

	// Linguistic Metadata
	public DbSet<JapanesePartOfSpeech> JapanesePartOfSpeeches => Set<JapanesePartOfSpeech>();
	public DbSet<DependencyRelationship> DependencyRelationships => Set<DependencyRelationship>();

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
