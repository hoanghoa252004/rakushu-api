using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Curator.Oov.ReviewOovCandidate;
using Rakushu.Domain.Entities.CuratorReview;
using Rakushu.Domain.Entities.Role;
using System;
using System.Threading;

namespace Rakushu.Api.Endpoints.Curator.ReviewOovCandidate;

internal sealed class ReviewOovCandidate : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorEndpoints()
			.MapPost("/{id:guid}/review", async (
				Guid id,
				[FromBody] ReviewOovCandidateRequestDto dto,
				ISender sender,
				ICurrentUserContext userContext,
				CancellationToken cancellationToken
			) =>
			{
				var curatorId = userContext.UserId?.Value ?? Guid.Empty;
				var command = new ReviewOovCandidateCommand(
					id,
					curatorId,
					dto.Decision,
					dto.EditedTerm,
					dto.EditedReading,
					dto.EditedPos,
					dto.EditedMeaning,
					dto.Comment
				);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("ReviewOovCandidate")
			.WithDescription("Submits curator review (ADAPT with adjustments or REJECT) for an OOV candidate.")
			.RequireAuthorization(policy => policy.RequireRole(
				DefaultSystemRoles.LinguisticCurator.ToString(),
				DefaultSystemRoles.SystemAdministrator.ToString()
			))
			.Produces<Guid>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record ReviewOovCandidateRequestDto(
	CuratorDecision Decision,
	string? EditedTerm = null,
	string? EditedReading = null,
	string? EditedPos = null,
	string? EditedMeaning = null,
	string? Comment = null
);
