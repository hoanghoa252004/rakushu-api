using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Payment;

namespace Rakushu.Persistence.Repositories;

public sealed class PaymentRepository : BaseRepository<Payment, PaymentId>, IPaymentRepository
{
	public PaymentRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Payment?> GetByOrderCodeAsync(string orderCode, CancellationToken cancellationToken = default)
	{
		return await _context.Payments
			.Include(p => p.Transactions)
			.Include(p => p.Subscription)
			.Include(p => p.Plan)
				.ThenInclude(pl => pl.PlanEntitlements)
			.SingleOrDefaultAsync(p => p.OrderCode == orderCode, cancellationToken);
	}

	public async Task<Payment?> GetByIdWithTransactionsAsync(PaymentId id, CancellationToken cancellationToken = default)
	{
		return await _context.Payments
			.Include(p => p.Transactions)
			.Include(p => p.Subscription)
			.Include(p => p.Plan)
				.ThenInclude(pl => pl.PlanEntitlements)
			.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
	}

	public async Task<bool> HasTransactionWithSepayIdAsync(long sepayId, CancellationToken cancellationToken = default)
	{
		return await _context.PaymentTransactions
			.AnyAsync(t => t.SepayId == sepayId, cancellationToken);
	}

	public async Task<List<Payment>> GetExpiredPendingPaymentsAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
	{
		return await _context.Payments
			.Include(p => p.Transactions)
			.Include(p => p.Subscription)
			.Where(p => p.Status == PaymentStatus.Pending && p.ExpiresAt <= now)
			.ToListAsync(cancellationToken);
	}

	public async Task<Payment?> GetActivePendingPaymentByUserIdAsync(Domain.Entities.User.UserId userId, DateTimeOffset now, CancellationToken cancellationToken = default)
	{
		return await _context.Payments
			.Include(p => p.Plan)
			.Include(p => p.Transactions)
			.FirstOrDefaultAsync(p => 
				p.UserId == userId && 
				p.Status == PaymentStatus.Pending && 
				p.ExpiresAt > now, 
				cancellationToken);
	}
}

