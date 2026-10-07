using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Usecases.Payment.Transaction.GetTransactionsByPaymentId;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Payment.Payment.GetPaymentById;

public sealed record PaymentDto(
	Guid Id,
	UserBasicDto? User,
	PlanBasicDto? Plan,
	decimal Amount,
	string Currency,
	string Status,
	DateTimeOffset ExpiredAt,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	IReadOnlyCollection<TransactionDto> Transactions
);

public sealed record PaymentBasicDto(
	Guid? Id,
	decimal? Amount,
	string? Currency,
	string? Status,
	DateTimeOffset? ExpiredAt,
	DateTimeOffset? CreatedAt,
	DateTimeOffset? UpdatedAt
);