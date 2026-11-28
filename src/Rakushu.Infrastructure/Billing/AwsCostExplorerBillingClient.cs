using System.Globalization;
using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;
using Rakushu.Application.Abstractions.Infrastructure.Billing;

namespace Rakushu.Infrastructure.Billing;

/// <summary>Reads real AWS billing data through AWS Cost Explorer (amounts in USD).</summary>
internal sealed class AwsCostExplorerBillingClient
{
	private const string CostMetric = "UnblendedCost";
	private const string UsageMetric = "UsageQuantity";

	private readonly IAmazonCostExplorer _client;

	public AwsCostExplorerBillingClient(IAmazonCostExplorer client)
	{
		_client = client;
	}

	public static string ToCostExplorerServiceName(ExternalService service) => service switch
	{
		ExternalService.AwsBedrock => "Amazon Bedrock",
		ExternalService.AwsEc2 => "Amazon Elastic Compute Cloud - Compute",
		ExternalService.AwsS3 => "Amazon Simple Storage Service",
		ExternalService.AwsMediaConvert => "AWS Elemental MediaConvert",
		ExternalService.AwsTranscribe => "Amazon Transcribe",
		ExternalService.AwsTranslate => "Amazon Translate",
		ExternalService.AwsRekognition => "Amazon Rekognition",
		ExternalService.AwsSes => "Amazon Simple Email Service",
		_ => throw new ArgumentOutOfRangeException(nameof(service), service, "Not an AWS service.")
	};

	public async Task<IReadOnlyList<CloudServiceMonthlyCost>> GetMonthlyCostsAsync(
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken)
	{
		var groups = await QueryAsync(year, months, "SERVICE", null, cancellationToken);

		return groups
			.Select(g => new CloudServiceMonthlyCost(CloudProvider.Aws, g.Key, g.Month, g.Cost, g.Currency))
			.ToList();
	}

	public async Task<IReadOnlyList<CloudUsageLine>> GetServiceUsageAsync(
		ExternalService service,
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken)
	{
		var filter = new Expression
		{
			Dimensions = new DimensionValues
			{
				Key = Dimension.SERVICE,
				Values = [ToCostExplorerServiceName(service)]
			}
		};

		var groups = await QueryAsync(year, months, "USAGE_TYPE", filter, cancellationToken);

		return groups
			.Select(g => new CloudUsageLine(g.Month, g.Key, g.Quantity, g.Unit, g.Cost, g.Currency))
			.ToList();
	}

	private async Task<List<(int Month, string Key, decimal Cost, string Currency, decimal Quantity, string? Unit)>> QueryAsync(
		int year,
		IReadOnlyCollection<int> months,
		string groupByKey,
		Expression? filter,
		CancellationToken cancellationToken)
	{
		var result = new List<(int, string, decimal, string, decimal, string?)>();

		var start = new DateTime(year, months.Min(), 1);
		var end = new DateTime(year, months.Max(), 1).AddMonths(1);

		// Cost Explorer does not accept periods in the future.
		var tomorrow = DateTime.UtcNow.Date.AddDays(1);
		if (end > tomorrow)
		{
			end = tomorrow;
		}

		if (start >= end)
		{
			return result;
		}

		string? nextToken = null;

		do
		{
			var request = new GetCostAndUsageRequest
			{
				TimePeriod = new DateInterval
				{
					Start = start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
					End = end.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
				},
				Granularity = Granularity.MONTHLY,
				Metrics = [CostMetric, UsageMetric],
				GroupBy = [new GroupDefinition { Type = GroupDefinitionType.DIMENSION, Key = groupByKey }],
				Filter = filter,
				NextPageToken = nextToken
			};

			var response = await _client.GetCostAndUsageAsync(request, cancellationToken);

			foreach (var period in response.ResultsByTime ?? [])
			{
				var month = DateTime.ParseExact(period.TimePeriod.Start, "yyyy-MM-dd", CultureInfo.InvariantCulture).Month;

				if (!months.Contains(month))
				{
					continue;
				}

				foreach (var group in period.Groups ?? [])
				{
					var key = group.Keys?.FirstOrDefault() ?? "Unknown";

					group.Metrics.TryGetValue(CostMetric, out var cost);
					group.Metrics.TryGetValue(UsageMetric, out var usage);

					result.Add((
						month,
						key,
						ParseDecimal(cost?.Amount),
						cost?.Unit ?? "USD",
						ParseDecimal(usage?.Amount),
						usage?.Unit));
				}
			}

			nextToken = response.NextPageToken;
		}
		while (!string.IsNullOrEmpty(nextToken));

		return result;
	}

	private static decimal ParseDecimal(string? value)
		=> decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0m;
}
