using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Payment;

namespace Rakushu.Persistence.Repositories;

public sealed class PaymentRepository : BaseRepository<Payment, PaymentId>, IPaymentRepository
{
	public PaymentRepository(RakushuDbContext context) : base(context)
	{
	}

	public override async Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken cancellationToken = default)
	{
		return await _context.Payments
			.Include(p => p.Transactions)
			.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
	}
}
