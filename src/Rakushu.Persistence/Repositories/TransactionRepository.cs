using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Payment.Transaction;

namespace Rakushu.Persistence.Repositories;

public sealed class TransactionRepository : BaseRepository<Transaction, TransactionId>, ITransactionRepository
{
	public TransactionRepository(RakushuDbContext context) : base(context) { }

	public async Task<Transaction?> GetByTxnRef(string txnRef, CancellationToken cancellationToken = default)
	{
		return await _context.Transactions
			.Include(t => t.Payment)
			.SingleOrDefaultAsync(t => t.TxnRef == txnRef, cancellationToken);
	}
}
