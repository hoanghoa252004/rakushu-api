namespace Rakushu.Application.Abstractions.Infrastructure.Payment;

public interface ISepayService
{
	int PaymentTimeoutInMinutes { get; }
	string BankName { get; }
	string AccountNumber { get; }
	string GenerateVietQrUrl(string orderCode, decimal amount, string? description = null);
	bool ValidateWebhook(string? signatureHeader, string? rawBody, string? timestampHeader);
}
