using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.ExpirePendingPayments;

public sealed record ExpirePendingPaymentsCommand : IRequest<Result<int>>;
