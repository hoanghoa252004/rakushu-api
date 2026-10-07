using MediatR;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.Payment.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IRequest<Result<PaymentDto>>;
