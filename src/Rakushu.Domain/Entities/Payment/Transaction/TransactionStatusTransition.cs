using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Payment.Transaction;


public static class TransactionStatusTransition
{
	public static readonly Dictionary<TransactionStatus, HashSet<TransactionStatus>> Transitions = new()
	{
		{
			TransactionStatus.Pending, new HashSet<TransactionStatus>
			{
				TransactionStatus.Successful,
				TransactionStatus.Failed,
				TransactionStatus.Cancelled,
				TransactionStatus.Expired
			}
		},
		{
			TransactionStatus.Successful, new HashSet<TransactionStatus>()
		},
		{
			TransactionStatus.Failed , new HashSet<TransactionStatus>()
		},
		{
			TransactionStatus.Cancelled , new HashSet<TransactionStatus>()
		},
		{
			TransactionStatus.Expired , new HashSet<TransactionStatus>()
		}
	};

	public static bool IsAllowed(TransactionStatus currentStatus, TransactionStatus newStatus)
	{
		if (!Transitions.ContainsKey(currentStatus))
		{
			return false;
		}

		return Transitions[currentStatus].Contains(newStatus);
	}

	public static string GetTransactionStatuses()
	{
		return string.Join(", ", Enum.GetNames(typeof(TransactionStatus)));
	}
}
