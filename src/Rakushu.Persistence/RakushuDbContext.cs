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
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Bunsetsu.BunsetsuDependencyRelationship;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Subtitle;
using Rakushu.Domain.Entities.Material.Subtitle.SubtitleSegment;
using Rakushu.Domain.Entities.Material.Token;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
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
using Rakushu.Domain.Entities.Srs;

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

	// Identity
	public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
	public DbSet<Interest> Interests => Set<Interest>();
	public DbSet<Profile> Profiles => Set<Profile>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<User> Users => Set<User>();



	// Knowledge
	public DbSet<ContentCategory> ContentCategories => Set<ContentCategory>();
	public DbSet<ContentKnowledgeCollection> ContentKnowledgeCollections => Set<ContentKnowledgeCollection>();
	public DbSet<KnowledgeCollection> KnowledgeCollections => Set<KnowledgeCollection>();
	public DbSet<KnowledgeMeaning> KnowledgeMeanings => Set<KnowledgeMeaning>();
	public DbSet<KnowledgeUnit> KnowledgeUnits => Set<KnowledgeUnit>();
	public DbSet<DependencyRelationship> DependencyRelationships => Set<DependencyRelationship>();
	public DbSet<JapanesePartOfSpeech> JapanesePartOfSpeeches => Set<JapanesePartOfSpeech>();
	public DbSet<ProficiencyLevel> ProficiencyLevels => Set<ProficiencyLevel>();

	

	// Material
	public DbSet<Bunsetsu> Bunsetsu => Set<Bunsetsu>();
	public DbSet<BunsetsuDependencyRelationship> BunsetsuDependencyRelationships => Set<BunsetsuDependencyRelationship>();
	public DbSet<LearningUnit> LearningUnits => Set<LearningUnit>();
	public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
	public DbSet<Series> Series => Set<Series>();
	public DbSet<Subtitle> Subtitles => Set<Subtitle>();
	public DbSet<SubtitleSegment> SubtitleSegments => Set<SubtitleSegment>();
	public DbSet<Token> Tokens => Set<Token>();
	public DbSet<Transcript> Transcripts => Set<Transcript>();
	public DbSet<TranscriptItem> TranscriptItems => Set<TranscriptItem>();
	public DbSet<TranscriptSegment> TranscriptSegments => Set<TranscriptSegment>();
	public DbSet<TranscriptSpeaker> TranscriptSpeakers => Set<TranscriptSpeaker>();
	public DbSet<Video> Videos => Set<Video>();

	// Subscriptions
	public DbSet<Entitlement> Entitlements => Set<Entitlement>();
	public DbSet<Feature> Features => Set<Feature>();
	public DbSet<Payment> Payments => Set<Payment>();
	public DbSet<Plan> Plans => Set<Plan>();
	public DbSet<Subscription> Subscriptions => Set<Subscription>();
	public DbSet<SubscriptionUsage> SubscriptionUsages => Set<SubscriptionUsage>();
	public DbSet<Transaction> Transactions => Set<Transaction>();

	// NotReviewedYet
	public DbSet<OovCandidate> OovCandidates => Set<OovCandidate>();
	public DbSet<CuratorReview> CuratorReviews => Set<CuratorReview>();
	public DbSet<DictionaryEntry> DictionaryEntries => Set<DictionaryEntry>();

	// SRS and Quiz
	public DbSet<SrsNote> SrsNotes => Set<SrsNote>();
	public DbSet<SrsCard> SrsCards => Set<SrsCard>();
	public DbSet<SrsCardState> SrsCardStates => Set<SrsCardState>();
	public DbSet<SrsReviewLog> SrsReviewLogs => Set<SrsReviewLog>();
	public DbSet<SrsSession> SrsSessions => Set<SrsSession>();
	public DbSet<SrsSessionItem> SrsSessionItems => Set<SrsSessionItem>();
	public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
	public DbSet<QuizAttemptItem> QuizAttemptItems => Set<QuizAttemptItem>();



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
