using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Create;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.Create;

internal sealed class CreateJapanesePartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapPost("/", async (
				[FromBody] CreateJapanesePartOfSpeechRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateJapanesePartOfSpeechCommand(
					dto.Code,
					dto.Name,
					dto.JapaneseName,
					dto.Description);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetJapanesePartOfSpeechById", id => new { id });
			})
			.WithName("CreateJapanesePartOfSpeech")
			.WithSummary("Admin")
			.WithDescription("Creates a new Japanese part of speech with Vietnamese and Japanese names.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<Guid>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateJapanesePartOfSpeechRequestDto(
	string Code,
	string Name,
	string JapaneseName,
	string? Description = null);
