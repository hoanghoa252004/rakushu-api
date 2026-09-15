using MediatR;
using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Events.DomainEvent;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Plan.PlanEntitlement;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.RefreshToken;
using Rakushu.Domain.Entities.User.Subscription;
using Rakushu.Domain.Entities.User.Subscription.SubscriptionUsage;

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

	public DbSet<Role> Roles => Set<Role>();
	public DbSet<User> Users => Set<User>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<Plan> Plans => Set<Plan>();
	public DbSet<Feature> Features => Set<Feature>();
	public DbSet<PlanEntitlement> PlanEntitlements => Set<PlanEntitlement>();
	public DbSet<Subscription> Subscriptions => Set<Subscription>();
	public DbSet<SubscriptionUsage> SubscriptionUsages => Set<SubscriptionUsage>();


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
