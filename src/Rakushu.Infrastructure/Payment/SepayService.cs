using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Infrastructure.Extensions.Options;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Rakushu.Infrastructure.Payment;

public sealed class SepayService : ISepayService
{
	private readonly SepaySettings _sepaySettings;
	private readonly PaymentSettings _paymentSettings;

	public SepayService(
		IOptions<SepaySettings> sepaySettings,
		IOptions<PaymentSettings> paymentSettings)
	{
		_sepaySettings = sepaySettings.Value;
		_paymentSettings = paymentSettings.Value;
	}

	public int PaymentTimeoutInMinutes => _paymentSettings.TimeoutInMinutes > 0 ? _paymentSettings.TimeoutInMinutes : 15;
	public string BankName => _sepaySettings.Bank;
	public string AccountNumber => _sepaySettings.AccountNumber;

	public string GenerateVietQrUrl(string orderCode, decimal amount, string? description = null)
	{
		var acc = WebUtility.UrlEncode(_sepaySettings.AccountNumber);
		var bank = WebUtility.UrlEncode(_sepaySettings.Bank);
		var des = WebUtility.UrlEncode(orderCode);
		var amountInt = (long)Math.Round(amount, MidpointRounding.AwayFromZero);

		return $"https://qr.sepay.vn/img?acc={acc}&bank={bank}&amount={amountInt}&des={des}&template=compact";
	}

	public bool ValidateWebhook(string? signatureHeader, string? rawBody, string? timestampHeader)
	{
		if (string.IsNullOrWhiteSpace(_sepaySettings.SecretKey) || string.IsNullOrWhiteSpace(signatureHeader) || string.IsNullOrWhiteSpace(rawBody))
		{
			return false;
		}

		var keyBytes = Encoding.UTF8.GetBytes(_sepaySettings.SecretKey.Trim());

		var cleanedSignature = signatureHeader.Trim().ToLowerInvariant();
		if (cleanedSignature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase))
		{
			cleanedSignature = cleanedSignature["sha256=".Length..].Trim();
		}

		// SePay standard format: ${timestamp}.${body}
		if (!string.IsNullOrWhiteSpace(timestampHeader))
		{
			var signedPayload = $"{timestampHeader.Trim()}.{rawBody}";
			using var hmacWithTimestamp = new HMACSHA256(keyBytes);
			var hashWithTimestamp = Convert.ToHexString(hmacWithTimestamp.ComputeHash(Encoding.UTF8.GetBytes(signedPayload))).ToLowerInvariant();
			if (CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(hashWithTimestamp), Encoding.UTF8.GetBytes(cleanedSignature)))
			{
				return true;
			}
		}

		// Fallback: hash only rawBody in case timestamp is not included
		using var hmacRaw = new HMACSHA256(keyBytes);
		var hashRaw = Convert.ToHexString(hmacRaw.ComputeHash(Encoding.UTF8.GetBytes(rawBody))).ToLowerInvariant();
		return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(hashRaw), Encoding.UTF8.GetBytes(cleanedSignature));
	}
}
