using MediatR;
using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Events.DomainEvent;
using Rakushu.Domain.Common.Results;
using Rakushu.Persistence.DbContext;

namespace Rakushu.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
	private readonly RakushuDbContext _context;
	private readonly IPublisher _publisher;

	public UnitOfWork(RakushuDbContext context, IPublisher publisher)
	{
		_context = context;
		_publisher = publisher;
	}

	public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
	{
		var strategy = _context.Database.CreateExecutionStrategy();

		return await strategy.ExecuteAsync(async () =>
		{
			await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
			try
			{
				var response = await action();

				// 1. If result is failure, rollback transaction
				if (response is Result result && result.IsFailure)
				{
					await transaction.RollbackAsync(cancellationToken);
					return response;
				}

				// 2. Cascade domain events loop
				do
				{
					var domainEvents = GetDomainEvents();
					if (domainEvents.Count > 0)
					{
						await DispatchDomainEventsAsync(domainEvents, cancellationToken);
					}
				} while (CheckDomainEventRemain());

				// 3. Save changes and commit transaction
				await _context.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);

				return response;
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				throw;
			}
		});
	}

	private List<IDomainEvent> GetDomainEvents()
	{
		var domainEventEntities = _context.ChangeTracker.Entries()
			.Select(e => e.Entity)
			.OfType<IHasDomainEvents>()
			.Where(e => e.DomainEvents.Count > 0)
			.ToList();

		var domainEvents = domainEventEntities
			.SelectMany(e => e.DomainEvents)
			.ToList();

		domainEventEntities.ForEach(e => e.ClearDomainEvents());

		return domainEvents;
	}

	private bool CheckDomainEventRemain()
	{
		return _context.ChangeTracker.Entries()
			.Select(e => e.Entity)
			.OfType<IHasDomainEvents>()
			.Any(e => e.DomainEvents.Count > 0);
	}

	private async Task DispatchDomainEventsAsync(List<IDomainEvent> domainEvents, CancellationToken cancellationToken)
	{
		foreach (var domainEvent in domainEvents)
		{
			await _publisher.Publish(domainEvent, cancellationToken);
		}
	}

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return _context.SaveChangesAsync(cancellationToken);
	}
}
