using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Rakushu.Application.Abstractions.Infrastructure.Billing;
using Rakushu.Application.Abstractions.Infrastructure.Configuration;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;

namespace Rakushu.Infrastructure.Billing;

/// <summary>Reads real Azure billing data through the Azure Cost Management Query REST API (service principal auth).</summary>
internal sealed class AzureCostManagementBillingClient
{
	private const string ManagementBaseUrl = "https://management.azure.com";
	private const string ApiVersion = "2023-03-01";
	private const string SpeechServiceName = "Cognitive Services";

	private readonly HttpClient _httpClient;
	private readonly ISystemConfigurationProvider _config;

	public AzureCostManagementBillingClient(HttpClient httpClient, ISystemConfigurationProvider config)
	{
		_httpClient = httpClient;
		_config = config;
	}

	public async Task<IReadOnlyList<CloudServiceMonthlyCost>> GetMonthlyCostsAsync(
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken)
	{
		var rows = await QueryAsync(year, months, "ServiceName", null, cancellationToken);

		return rows
			.Select(r => new CloudServiceMonthlyCost(CloudProvider.Azure, r.Key, r.Month, r.Cost, r.Currency))
			.ToList();
	}

	public async Task<IReadOnlyList<CloudUsageLine>> GetServiceUsageAsync(
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken)
	{
		var filter = new
		{
			dimensions = new { name = "ServiceName", @operator = "In", values = new[] { SpeechServiceName } }
		};

		var rows = await QueryAsync(year, months, "Meter", filter, cancellationToken);

		return rows
			.Select(r => new CloudUsageLine(r.Month, r.Key, r.Quantity, null, r.Cost, r.Currency))
			.ToList();
	}

	private async Task<List<(int Month, string Key, decimal Cost, string Currency, decimal Quantity)>> QueryAsync(
		int year,
		IReadOnlyCollection<int> months,
		string groupBy,
		object? filter,
		CancellationToken cancellationToken)
	{
		var result = new List<(int, string, decimal, string, decimal)>();

		var subscriptionId = await _config.GetStringAsync(SystemConfigurationKey.AzureSettings_SubscriptionId, cancellationToken);
		var accessToken = await GetAccessTokenAsync(cancellationToken);

		var from = new DateTime(year, months.Min(), 1, 0, 0, 0, DateTimeKind.Utc);
		var to = new DateTime(year, months.Max(), 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).AddSeconds(-1);

		var body = new Dictionary<string, object?>
		{
			["type"] = "ActualCost",
			["timeframe"] = "Custom",
			["timePeriod"] = new { from = from.ToString("o"), to = to.ToString("o") },
			["dataset"] = new Dictionary<string, object?>
			{
				["granularity"] = "Monthly",
				["aggregation"] = new
				{
					totalCost = new { name = "Cost", function = "Sum" },
					totalQuantity = new { name = "UsageQuantity", function = "Sum" }
				},
				["grouping"] = new[] { new { type = "Dimension", name = groupBy } },
				["filter"] = filter
			}
		};

		string? url = $"{ManagementBaseUrl}/subscriptions/{subscriptionId}/providers/Microsoft.CostManagement/query?api-version={ApiVersion}";

		while (!string.IsNullOrEmpty(url))
		{
			using var request = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = JsonContent.Create(body, options: new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull })
			};
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			using var response = await _httpClient.SendAsync(request, cancellationToken);
			response.EnsureSuccessStatusCode();

			using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
			var properties = json.RootElement.GetProperty("properties");

			var columns = properties.GetProperty("columns")
				.EnumerateArray()
				.Select((c, i) => (Name: c.GetProperty("name").GetString() ?? string.Empty, Index: i))
				.ToDictionary(c => c.Name, c => c.Index, StringComparer.OrdinalIgnoreCase);

			foreach (var row in properties.GetProperty("rows").EnumerateArray())
			{
				var values = row.EnumerateArray().ToList();

				var month = ReadMonth(values[columns["BillingMonth"]]);
				if (!months.Contains(month))
				{
					continue;
				}

				result.Add((
					month,
					columns.TryGetValue(groupBy, out var keyIdx) ? values[keyIdx].ToString() : "Unknown",
					columns.TryGetValue("Cost", out var costIdx) ? values[costIdx].GetDecimal() : 0m,
					columns.TryGetValue("Currency", out var curIdx) ? values[curIdx].GetString() ?? "USD" : "USD",
					columns.TryGetValue("UsageQuantity", out var qtyIdx) ? values[qtyIdx].GetDecimal() : 0m));
			}

			url = properties.TryGetProperty("nextLink", out var next) && next.ValueKind == JsonValueKind.String
				? next.GetString()
				: null;
		}

		return result;
	}

	private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
	{
		var tenantId = await _config.GetStringAsync(SystemConfigurationKey.AzureSettings_TenantId, cancellationToken);
		var clientId = await _config.GetStringAsync(SystemConfigurationKey.AzureSettings_ClientId, cancellationToken);
		var clientSecret = await _config.GetStringAsync(SystemConfigurationKey.AzureSettings_ClientSecret, cancellationToken);

		using var content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			["grant_type"] = "client_credentials",
			["client_id"] = clientId,
			["client_secret"] = clientSecret,
			["scope"] = $"{ManagementBaseUrl}/.default"
		});

		using var response = await _httpClient.PostAsync(
			$"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token", content, cancellationToken);
		response.EnsureSuccessStatusCode();

		using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);

		return json.RootElement.GetProperty("access_token").GetString()
			?? throw new InvalidOperationException("Azure access token was not returned.");
	}

	private static int ReadMonth(JsonElement value)
	{
		// BillingMonth is returned as "2026-04-01T00:00:00" (string) or 20260401 (number).
		if (value.ValueKind == JsonValueKind.Number)
		{
			return value.GetInt32() / 100 % 100;
		}

		return DateTime.Parse(value.GetString()!, CultureInfo.InvariantCulture).Month;
	}
}
