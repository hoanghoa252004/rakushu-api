using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Billing;
using Rakushu.Application.Abstractions.Infrastructure.Configuration;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Parsing;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;

namespace Rakushu.Application.Usecases.Analytics.Finance.GetExternalServiceUsage;

/// <param name="Service">Required. One of <see cref="ExternalService"/> names (e.g. AwsBedrock, AwsEc2, AzureSpeech).</param>
/// <param name="Months">Months of the year to include (1..12). Empty = all 12 months.</param>
public sealed record GetExternalServiceUsageQuery(
	string Service,
	int Year,
	IReadOnlyCollection<int>? Months = null) : IRequest<Result<ExternalServiceUsageDto>>;

internal sealed class GetExternalServiceUsageValidator : AbstractValidator<GetExternalServiceUsageQuery>
{
	public GetExternalServiceUsageValidator()
	{
		RuleFor(x => x.Service)
			.NotEmpty()
			.WithMessage("Service is required.");

		RuleFor(x => x.Year)
			.InclusiveBetween(2000, 2100)
			.WithMessage("Year must be between 2000 and 2100.");

		RuleForEach(x => x.Months)
			.InclusiveBetween(1, 12)
			.WithMessage("Month must be between 1 and 12.");
	}
}

internal sealed class GetExternalServiceUsageHandler
	: IRequestHandler<GetExternalServiceUsageQuery, Result<ExternalServiceUsageDto>>
{
	private readonly IFinanceAnalyticsQueryService _queryService;
	private readonly ICloudBillingService _billingService;
	private readonly ISystemConfigurationProvider _config;
	private readonly ISystemClock _clock;

	public GetExternalServiceUsageHandler(
		IFinanceAnalyticsQueryService queryService,
		ICloudBillingService billingService,
		ISystemConfigurationProvider config,
		ISystemClock clock)
	{
		_queryService = queryService;
		_billingService = billingService;
		_config = config;
		_clock = clock;
	}

	public async Task<Result<ExternalServiceUsageDto>> Handle(GetExternalServiceUsageQuery request, CancellationToken cancellationToken)
	{
		var parsed = EnumParser.Parse<ExternalService>(request.Service, "service");
		if (parsed.IsFailure)
		{
			return Result.Failure<ExternalServiceUsageDto>(parsed.Error);
		}

		var service = parsed.Value;
		var months = FinanceHelper.NormalizeMonths(request.Months);
		var rate = await FinanceHelper.GetUsdToVndRateAsync(_config, cancellationToken);
		var provider = service == ExternalService.AzureSpeech ? CloudProvider.Azure : CloudProvider.Aws;

		var usageLines = await _billingService.GetServiceUsageAsync(service, request.Year, months, cancellationToken);

		// Internal usage tracked by the system (jobs / AI messages).
		var jobServiceName = ToJobServiceName(service);
		IReadOnlyList<MonthlyJobUsageRow> jobRows = [];
		IReadOnlyList<MonthlyModelUsageRow> modelRows = [];
		DateTimeOffset? firstUsedAt = null;

		if (jobServiceName is not null)
		{
			jobRows = await _queryService.GetMonthlyJobUsageAsync(jobServiceName, request.Year, months, cancellationToken);
			firstUsedAt = await _queryService.GetFirstJobUsedAtAsync(jobServiceName, cancellationToken);
		}

		if (service == ExternalService.AwsBedrock)
		{
			modelRows = await _queryService.GetMonthlyAiModelUsageAsync(request.Year, months, cancellationToken);
			firstUsedAt = await _queryService.GetFirstAiUsedAtAsync(cancellationToken);
		}

		var originalCurrency = usageLines.FirstOrDefault()?.Currency ?? "USD";

		var monthly = months.Select(month =>
		{
			var lines = usageLines.Where(l => l.Month == month).ToList();
			var jobs = jobServiceName is null ? null : BuildJobs(jobRows.Where(j => j.Month == month));
			var originalCost = lines.Sum(l => l.Cost);

			return new MonthlyServiceUsageDto(
				month,
				FinanceHelper.ToVnd(originalCost, originalCurrency, rate),
				originalCost,
				jobs,
				BuildModels(modelRows.Where(m => m.Month == month)),
				BuildUsageTypes(lines, rate));
		}).ToList();

		var totalOriginal = usageLines.Sum(l => l.Cost);
		var usageTypes = BuildUsageTypes(usageLines, rate);

		return Result.Success(new ExternalServiceUsageDto(
			service.ToString(),
			provider.ToString(),
			ToBillingServiceName(service),
			request.Year,
			months,
			FinanceHelper.BaseCurrency,
			rate,
			firstUsedAt,
			firstUsedAt is null ? null : (int)(_clock.UtcNow - firstUsedAt.Value).TotalDays,
			FinanceHelper.ToVnd(totalOriginal, originalCurrency, rate),
			totalOriginal,
			originalCurrency,
			await GetConfiguredResourcesAsync(service, cancellationToken),
			GetUsedResources(service, usageTypes, modelRows),
			jobServiceName is null ? null : BuildJobs(jobRows),
			BuildModels(modelRows),
			usageTypes,
			monthly));
	}

	private static string? ToJobServiceName(ExternalService service) => service switch
	{
		ExternalService.AwsTranscribe => ProcessingJobServices.AwsTranscribe,
		ExternalService.AwsRekognition => ProcessingJobServices.AwsRekognition,
		ExternalService.AwsTranslate => ProcessingJobServices.AwsTranslate,
		ExternalService.AwsMediaConvert => ProcessingJobServices.AwsMediaConvert,
		ExternalService.AzureSpeech => ProcessingJobServices.AzureSpeech,
		_ => null
	};

	private static string ToBillingServiceName(ExternalService service) => service switch
	{
		ExternalService.AwsBedrock => "Amazon Bedrock",
		ExternalService.AwsEc2 => "Amazon Elastic Compute Cloud - Compute",
		ExternalService.AwsS3 => "Amazon Simple Storage Service",
		ExternalService.AwsMediaConvert => "AWS Elemental MediaConvert",
		ExternalService.AwsTranscribe => "Amazon Transcribe",
		ExternalService.AwsTranslate => "Amazon Translate",
		ExternalService.AwsRekognition => "Amazon Rekognition",
		ExternalService.AwsSes => "Amazon Simple Email Service",
		ExternalService.AzureSpeech => "Cognitive Services (Speech)",
		_ => service.ToString()
	};

	private async Task<IReadOnlyList<string>> GetConfiguredResourcesAsync(ExternalService service, CancellationToken cancellationToken)
	{
		var keys = service switch
		{
			ExternalService.AwsBedrock => new[]
			{
				SystemConfigurationKey.AwsSettings_BedrockChatModelId,
				SystemConfigurationKey.AwsSettings_BedrockModelId
			},
			ExternalService.AwsS3 => [SystemConfigurationKey.AwsSettings_BucketName],
			ExternalService.AwsSes => [SystemConfigurationKey.AwsSettings_SenderEmail],
			ExternalService.AzureSpeech => [SystemConfigurationKey.AzureSettings_AzureSpeechRegion],
			_ => []
		};

		var result = new List<string>();
		foreach (var key in keys)
		{
			var value = await _config.GetStringAsync(key, cancellationToken);
			if (!string.IsNullOrWhiteSpace(value))
			{
				result.Add($"{key.ToString().Split('_')[^1]}: {value}");
			}
		}

		return result;
	}

	// Models (Bedrock) or instance types (EC2, e.g. "APS1-BoxUsage:t3.medium" -> "t3.medium") actually used in the period.
	private static IReadOnlyList<string> GetUsedResources(
		ExternalService service,
		IReadOnlyList<UsageTypeDto> usageTypes,
		IReadOnlyList<MonthlyModelUsageRow> modelRows)
	{
		return service switch
		{
			ExternalService.AwsBedrock => modelRows.Select(m => m.ModelId).Distinct().ToList(),
			ExternalService.AwsEc2 => usageTypes
				.Where(u => u.UsageType.Contains("BoxUsage", StringComparison.OrdinalIgnoreCase))
				.Select(u => u.UsageType.Contains(':') ? u.UsageType[(u.UsageType.LastIndexOf(':') + 1)..] : "t2.micro / default")
				.Distinct()
				.ToList(),
			_ => []
		};
	}

	private static ServiceJobStatisticsDto BuildJobs(IEnumerable<MonthlyJobUsageRow> rows)
	{
		var list = rows.ToList();
		var total = list.Sum(r => r.TotalJobs);
		var seconds = Math.Round(list.Sum(r => r.ProcessingSeconds), 2);

		return new ServiceJobStatisticsDto(
			total,
			list.Sum(r => r.SucceededJobs),
			list.Sum(r => r.FailedJobs),
			seconds,
			total == 0 ? 0 : Math.Round(seconds / total, 2));
	}

	private static IReadOnlyList<AiModelUsageDto> BuildModels(IEnumerable<MonthlyModelUsageRow> rows)
		=> rows
			.GroupBy(r => r.ModelId)
			.Select(g => new AiModelUsageDto(g.Key, g.Sum(x => x.Requests), g.Sum(x => x.InputTokens), g.Sum(x => x.OutputTokens)))
			.OrderByDescending(m => m.Requests)
			.ToList();

	private static IReadOnlyList<UsageTypeDto> BuildUsageTypes(IEnumerable<CloudUsageLine> lines, decimal rate)
		=> lines
			.GroupBy(l => new { l.UsageType, l.Unit, l.Currency })
			.Select(g =>
			{
				var original = g.Sum(x => x.Cost);
				return new UsageTypeDto(
					g.Key.UsageType,
					g.Sum(x => x.Quantity),
					g.Key.Unit,
					FinanceHelper.ToVnd(original, g.Key.Currency, rate),
					original);
			})
			.OrderByDescending(u => u.Cost)
			.ToList();
}
