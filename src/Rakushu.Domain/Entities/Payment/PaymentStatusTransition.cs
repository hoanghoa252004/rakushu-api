using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Payment;

public static class PaymentStatusTransition
{
	public static readonly Dictionary<PaymentStatus, HashSet<PaymentStatus>> Transitions = new()
	{
		{
			PaymentStatus.Pending, new HashSet<PaymentStatus>
			{
				PaymentStatus.Completed,
				PaymentStatus.Failed,
				PaymentStatus.Cancelled,
				PaymentStatus.Expired
			}
		},
		{
			PaymentStatus.Completed, new HashSet<PaymentStatus>()
		},
		{
			PaymentStatus.Failed , new HashSet<PaymentStatus>()
		},
		{
			PaymentStatus.Cancelled , new HashSet<PaymentStatus>()
		},
		{
			PaymentStatus.Expired , new HashSet<PaymentStatus>()
		}
	};

	public static bool IsAllowed(PaymentStatus currentStatus, PaymentStatus newStatus)
	{
		if (!Transitions.ContainsKey(currentStatus))
		{
			return false;
		}

		return Transitions[currentStatus].Contains(newStatus);
	}

	public static string GetPaymentStatuses()
	{
		return string.Join(", ", Enum.GetNames(typeof(PaymentStatus)));
	}
}