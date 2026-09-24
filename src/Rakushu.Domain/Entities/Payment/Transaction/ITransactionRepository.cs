using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Payment.Transaction;

namespace Rakushu.Domain.Entities.Payment.Transaction;

public interface ITransactionRepository : IBaseRepository<Transaction, TransactionId>
{
	Task<Transaction?> GetByTxnRef(string txnRef, CancellationToken cancellationToken = default);
}
