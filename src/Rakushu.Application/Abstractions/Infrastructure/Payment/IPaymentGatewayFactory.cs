using Rakushu.Domain.Entities.Subscriptions.Payment;

namespace Rakushu.Application.Abstractions.Infrastructure.Payment;

public interface IPaymentGatewayFactory
{
	IPaymentService GetPaymentService(Provider provider);
}
