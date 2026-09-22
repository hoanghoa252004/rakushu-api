using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IRequest<Result<PaymentDto>>;
