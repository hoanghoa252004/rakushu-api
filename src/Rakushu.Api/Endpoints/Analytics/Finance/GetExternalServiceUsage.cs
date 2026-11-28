using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Finance;
using Rakushu.Application.Usecases.Analytics.Finance.GetExternalServiceUsage;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Finance;

internal sealed class GetExternalServiceUsage : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapFinanceAnalyticsEndpoints()
			.MapGet("/services/{service}", async (
				string service,
				[FromQuery] int year,
				[FromQuery] int[]? months,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(
					new GetExternalServiceUsageQuery(service, year, months),
					cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetExternalServiceUsage")
			.WithSummary("Administrator")
			.WithDescription(
				"Returns usage detail of one external service for the selected months of a year: cost (VND + original currency), " +
				"usage types/quantities from the billing API, models/instances in use, processed jobs and duration, first usage date.\n\n" +
				"Available values for `service` (case-insensitive, required):\n" +
				"- `AwsBedrock` – Amazon Bedrock (AI assistant models, token usage)\n" +
				"- `AwsEc2` – Amazon EC2 (compute instances)\n" +
				"- `AwsS3` – Amazon S3 (storage)\n" +
				"- `AwsMediaConvert` – AWS Elemental MediaConvert (video/audio conversion jobs)\n" +
				"- `AwsTranscribe` – Amazon Transcribe (speech-to-text jobs)\n" +
				"- `AwsTranslate` – Amazon Translate (transcript translation jobs)\n" +
				"- `AwsRekognition` – Amazon Rekognition (content moderation jobs)\n" +
				"- `AwsSes` – Amazon SES (email sending)\n" +
				"- `AzureSpeech` – Azure Speech Service (pronunciation assessment)\n\n" +
				"Example: /api/analytics/finance/services/AwsBedrock?year=2026&months=4&months=5&months=6 (omit months for all 12 months).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			.Produces<ExternalServiceUsageDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
