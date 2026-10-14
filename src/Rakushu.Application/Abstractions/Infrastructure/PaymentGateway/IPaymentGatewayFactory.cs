using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Domain.Entities.Subscriptions.Payment;

namespace Rakushu.Application.Abstractions.Infrastructure.PaymentGateway;

public interface IPaymentGatewayFactory
{
	IPaymentService GetPaymentService(Provider provider);
}
