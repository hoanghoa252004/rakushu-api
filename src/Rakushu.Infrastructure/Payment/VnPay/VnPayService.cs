using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Configuration;
using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;
using Rakushu.Domain.Entities.Subscriptions.Payment;

namespace Rakushu.Infrastructure.Payment.VnPay;

internal sealed class VnPayService : IPaymentService
{
	private readonly ISystemConfigurationProvider _config;
	private readonly ISystemClock _systemClock;

	public VnPayService(
		ISystemConfigurationProvider config,
		ISystemClock systemClock
		)
	{
		_config = config;
		_systemClock = systemClock;
	}

	public Provider Provider => Provider.VNPAY;

	public Result<string> CreatePaymentUrl(CreatePaymentUrlParams parameters, CancellationToken cancellationToken = default)
	{
		var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_config.GetString(SystemConfigurationKey.VnPaySettings_TimeZoneId));
		var createdAt = TimeZoneInfo.ConvertTime(parameters.CreatedAt, timeZoneById);
		var expiredAt = TimeZoneInfo.ConvertTime(parameters.ExpiredAt, timeZoneById);

		var pay = new VnPayLibrary();
		var amount = ((long)parameters.Amount * 100).ToString();

		pay.AddRequestData("vnp_IpAddr", parameters.IpAddress);
		pay.AddRequestData("vnp_Command", _config.GetString(SystemConfigurationKey.VnPaySettings_Command));
		pay.AddRequestData("vnp_Version", _config.GetString(SystemConfigurationKey.VnPaySettings_Version));
		pay.AddRequestData("vnp_TmnCode", _config.GetString(SystemConfigurationKey.VnPaySettings_TmnCode));
		pay.AddRequestData("vnp_Locale", _config.GetString(SystemConfigurationKey.VnPaySettings_Locale));
		pay.AddRequestData("vnp_CurrCode", _config.GetString(SystemConfigurationKey.VnPaySettings_CurrCode));
		pay.AddRequestData("vnp_ReturnUrl", _config.GetString(SystemConfigurationKey.VnPaySettings_ReturnUrl));
		pay.AddRequestData("vnp_OrderType", _config.GetString(SystemConfigurationKey.VnPaySettings_OrderType));
		pay.AddRequestData("vnp_BankCode", _config.GetString(SystemConfigurationKey.VnPaySettings_BankCode));
		pay.AddRequestData("vnp_CreateDate", createdAt.ToString("yyyyMMddHHmmss"));
		pay.AddRequestData("vnp_ExpireDate", expiredAt.ToString("yyyyMMddHHmmss"));

		pay.AddRequestData("vnp_Amount", amount);
		pay.AddRequestData("vnp_OrderInfo", parameters.Description);
		pay.AddRequestData("vnp_TxnRef", parameters.TxnRef);

		var baseUrl = _config.GetString(SystemConfigurationKey.VnPaySettings_BaseUrl);
		var hashSecret = _config.GetString(SystemConfigurationKey.VnPaySettings_HashSecret);
		var paymentUrl = pay.CreateRequestUrl(baseUrl, hashSecret);

		return Result.Success(paymentUrl);
	}

	public DateTimeOffset GetPaymentExpiration()
		=> _systemClock.UtcNow.AddSeconds(_config.GetInt(SystemConfigurationKey.VnPaySettings_PaymentExpiredInSeconds));

	public Result<string?> GetResponseData(IReadOnlyDictionary<string, string> parameters, string key, CancellationToken cancellationToken = default)
	{
		if (parameters.TryGetValue(key, out var value))
		{
			return Result.Success<string?>(value);
		}

		return Result.Failure<string?>(Error.None());
	}

	public DateTimeOffset GetTransactionExpiration()
		=> _systemClock.UtcNow.AddSeconds(_config.GetInt(SystemConfigurationKey.VnPaySettings_TransactionExpiredInSeconds));

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

			var hashSecret = _config.GetString(SystemConfigurationKey.VnPaySettings_HashSecret);

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
