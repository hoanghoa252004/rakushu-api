using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Infrastructure.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Payment.VnPay;

internal sealed class VnPayService : IPaymentService
{
	private readonly VnPaySettings _vnPaySettings;
	private readonly ISystemClock _systemClock;

	public VnPayService(
		IOptions<VnPaySettings> options,
		ISystemClock systemClock
		)
	{ 
		_vnPaySettings = options.Value;
		_systemClock = systemClock;
	}

	public Provider Provider => Provider.VNPAY;

	public Result<string> CreatePaymentUrl(CreatePaymentUrlParams parameters, CancellationToken cancellationToken = default)
	{
		var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_vnPaySettings.TimeZoneId);
		var createdAt = TimeZoneInfo.ConvertTimeFromUtc(parameters.CreatedAt, timeZoneById);
		var expiredAt = TimeZoneInfo.ConvertTimeFromUtc(parameters.ExpiredAt, timeZoneById);
		var pay = new VnPayLibrary();
		var amount = ((long)parameters.Amount * 100).ToString();

		pay.AddRequestData("vnp_IpAddr", parameters.IpAddress);
		pay.AddRequestData("vnp_Command", _vnPaySettings.Command);
		pay.AddRequestData("vnp_Version", _vnPaySettings.Version);
		pay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
		pay.AddRequestData("vnp_Locale", _vnPaySettings.Locale);
		pay.AddRequestData("vnp_CurrCode", _vnPaySettings.CurrCode);
		pay.AddRequestData("vnp_ReturnUrl", _vnPaySettings.ReturnUrl);
		pay.AddRequestData("vnp_OrderType", _vnPaySettings.OrderType);
		pay.AddRequestData("vnp_BankCode", _vnPaySettings.BankCode);
		pay.AddRequestData("vnp_CreateDate", createdAt.ToString("yyyyMMddHHmmss"));
		pay.AddRequestData("vnp_ExpireDate", expiredAt.ToString("yyyyMMddHHmmss"));

		pay.AddRequestData("vnp_Amount", amount);
		pay.AddRequestData("vnp_OrderInfo", parameters.Description);
		pay.AddRequestData("vnp_TxnRef", parameters.TxnRef);

		var baseUrl = _vnPaySettings.BaseUrl;
		var hashSecret = _vnPaySettings.HashSecret;
		var paymentUrl = pay.CreateRequestUrl(baseUrl, hashSecret);

		return Result.Success(paymentUrl);
	}

	public DateTimeOffset GetPaymentExpiration()
		=> _systemClock.UtcNow.AddSeconds(_vnPaySettings.PaymentExpiredInSeconds);

	public Result<string?> GetResponseData(IReadOnlyDictionary<string, string> parameters, string key, CancellationToken cancellationToken = default)
	{
		if (parameters.TryGetValue(key, out var value))
		{
			return Result.Success<string?>(value);
		}

		return Result.Failure<string?>(Error.None());
	}

	public DateTimeOffset GetTransactionExpiration()
		=> _systemClock.UtcNow.AddSeconds(_vnPaySettings.TransactionExpiredInSeconds);

	public Result ValidateSignature(IReadOnlyDictionary<string, string> parameters, string secureHash, CancellationToken cancellationToken = default)
	{
		try
		{
			var vnpay = new VnPayLibrary();

			// Add all vnp_* parameters to the validator
			foreach (var (key, value) in parameters)
			{
				if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
				{
					vnpay.AddResponseData(key, value.ToString());
				}
			}

			// Get hash secret from configuration
			var hashSecret = _vnPaySettings.HashSecret;

			// Validate the signature
			return vnpay.ValidateSignature(secureHash, hashSecret) ?
				Result.Success() :
				Result.Failure(PaymentError.InvalidSignature);
		}
		catch
		{
			return Result.Failure(PaymentError.InvalidSignature);
		}
	}
}
