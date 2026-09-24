using Rakushu.Domain.Entities.Payment.Transaction;

namespace Rakushu.Persistence.Repositories;

public sealed class TransactionRepository : BaseRepository<Transaction, TransactionId>, ITransactionRepository
{
	public TransactionRepository(RakushuDbContext context) : base(context)
	{
	}
}
