using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Application.Abstractions.Infrastructure.PaymentGateway;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Infrastructure.Payment.VnPay;

namespace Rakushu.Infrastructure.Payment;

internal sealed class PaymentGatewayFactory : IPaymentGatewayFactory
{
	private readonly VnPayService _vnPayService;
	//private readonly SePayGatewayService _sePayService;

	public PaymentGatewayFactory(
		VnPayService vnPayService
		//SePayGatewayService sePayService
		)
	{
		_vnPayService = vnPayService;
		//_sePayService = sePayService;
	}

	public IPaymentService GetPaymentService(Provider provider)
	{
		return provider switch
		{
			Provider.VNPAY => _vnPayService,
			//Provider.SEPAY => _sePayService,
			_ => throw new ArgumentException($"Unsupported payment provider: {provider}")
		};
	}
}
