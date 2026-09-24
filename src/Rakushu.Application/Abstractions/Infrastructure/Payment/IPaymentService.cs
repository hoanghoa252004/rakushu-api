namespace Rakushu.Application.Abstractions.Infrastructure.Payment;

public interface IPaymentService
{
	int PaymentTimeoutInMinutes { get; }
	int TransactionTimeoutInMinutes { get; }
	string BankName { get; }
	string AccountNumber { get; }
	string GenerateVietQrUrl(string orderCode, decimal amount, string? description = null);
	bool ValidateWebhook(string? signatureHeader, string? rawBody, string? timestampHeader);
}
