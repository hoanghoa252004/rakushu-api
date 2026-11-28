using Rakushu.Application.Abstractions.Infrastructure.Billing;

namespace Rakushu.Infrastructure.Billing;

internal sealed class CloudBillingService : ICloudBillingService
{
	private readonly AwsCostExplorerBillingClient _aws;
	private readonly AzureCostManagementBillingClient _azure;

	public CloudBillingService(AwsCostExplorerBillingClient aws, AzureCostManagementBillingClient azure)
	{
		_aws = aws;
		_azure = azure;
	}

	public async Task<IReadOnlyList<CloudServiceMonthlyCost>> GetMonthlyCostsAsync(
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default)
	{
		var awsTask = _aws.GetMonthlyCostsAsync(year, months, cancellationToken);
		var azureTask = _azure.GetMonthlyCostsAsync(year, months, cancellationToken);

		await Task.WhenAll(awsTask, azureTask);

		return [.. awsTask.Result, .. azureTask.Result];
	}

	public Task<IReadOnlyList<CloudUsageLine>> GetServiceUsageAsync(
		ExternalService service,
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default)
	{
		return service == ExternalService.AzureSpeech
			? _azure.GetServiceUsageAsync(year, months, cancellationToken)
			: _aws.GetServiceUsageAsync(service, year, months, cancellationToken);
	}
}
