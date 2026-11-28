namespace Rakushu.Application.Abstractions.Infrastructure.Billing;

public enum CloudProvider
{
	Aws,
	Azure
}

public enum ExternalService
{
	AwsBedrock,
	AwsEc2,
	AwsS3,
	AwsMediaConvert,
	AwsTranscribe,
	AwsTranslate,
	AwsRekognition,
	AwsSes,
	AzureSpeech
}

/// <summary>Cost of one cloud service in one month, in the billing currency of the provider.</summary>
public sealed record CloudServiceMonthlyCost(
	CloudProvider Provider,
	string ServiceName,
	int Month,
	decimal Cost,
	string Currency);

/// <summary>Usage of one usage type / meter of a service in one month.</summary>
public sealed record CloudUsageLine(
	int Month,
	string UsageType,
	decimal Quantity,
	string? Unit,
	decimal Cost,
	string Currency);

public interface ICloudBillingService
{
	/// <summary>Costs of every AWS and Azure service grouped by service and month.</summary>
	Task<IReadOnlyList<CloudServiceMonthlyCost>> GetMonthlyCostsAsync(
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default);

	/// <summary>Usage lines (usage type / meter) of a specific external service grouped by month.</summary>
	Task<IReadOnlyList<CloudUsageLine>> GetServiceUsageAsync(
		ExternalService service,
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default);
}
