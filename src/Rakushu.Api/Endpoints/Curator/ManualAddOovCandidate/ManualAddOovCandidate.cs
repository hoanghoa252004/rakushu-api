using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Curator.Oov.ManualAddOovCandidate;
using Rakushu.Domain.Entities.Role;
using System;
using System.Threading;

namespace Rakushu.Api.Endpoints.Curator.ManualAddOovCandidate;

internal sealed class ManualAddOovCandidate : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorEndpoints()
			.MapPost("/{id:guid}/manual-add", async (
				Guid id,
				[FromBody] ManualAddOovCandidateRequestDto dto,
				ISender sender,
				ICurrentUserContext userContext,
				CancellationToken cancellationToken
			) =>
			{
				var curatorId = userContext.UserId?.Value ?? Guid.Empty;
				var command = new ManualAddOovCandidateCommand(
					id,
					curatorId,
					dto.Term,
					dto.Reading,
					dto.Pos,
					dto.Meaning,
					dto.Comment
				);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("ManualAddOovCandidate")
			.WithDescription("Allows curator to reject AI proposal and manually input verified vocabulary attributes.")
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

public sealed record ManualAddOovCandidateRequestDto(
	string Term,
	string Reading,
	string Pos,
	string Meaning,
	string? Comment = null
);
