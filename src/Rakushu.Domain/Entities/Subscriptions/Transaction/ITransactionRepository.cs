using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Subscriptions.Transaction;

public interface ITransactionRepository : IBaseRepository<Transaction, TransactionId>
{
	Task<Transaction?> GetByTxnRef(string txnRef, CancellationToken cancellationToken = default);
}
