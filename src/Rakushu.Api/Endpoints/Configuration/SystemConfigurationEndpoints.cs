using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Configuration.SystemConfiguration;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Configuration;

internal sealed class SystemConfigurationEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/system-configurations")
			.WithTags("SystemConfiguration")
			.WithGroupName("configuration")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator));

		group.MapGet("/", async (ISender sender, CancellationToken ct) =>
			{
				var result = await sender.Send(new GetSystemConfigurationsQuery(), ct);
				return result.MatchOk();
			})
			.WithName("GetSystemConfigurations")
			.WithSummary("Admin")
			.WithDescription("Gets all system configurations with their version history.")
			.Produces<IReadOnlyCollection<SystemConfigurationGroupDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);

		group.MapGet("/{id:guid}", async ([FromRoute] Guid id, ISender sender, CancellationToken ct) =>
			{
				var result = await sender.Send(new GetSystemConfigurationByIdQuery(id), ct);
				return result.MatchOk();
			})
			.WithName("GetSystemConfigurationById")
			.WithSummary("Admin")
			.WithDescription("Gets a system configuration with its version history.")
			.Produces<SystemConfigurationDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);

		group.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateSystemConfigurationRequestDto dto,
				ISender sender,
				CancellationToken ct) =>
			{
				var result = await sender.Send(new UpdateSystemConfigurationCommand(id, dto.ConfigValue, dto.Version), ct);
				return result.MatchOk();
			})
			.WithName("UpdateSystemConfiguration")
			.WithSummary("Admin")
			.WithDescription("Provide ConfigValue to create a new active version, or Version to switch the active version (only one of them).")
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record UpdateSystemConfigurationRequestDto(string? ConfigValue = null, int? Version = null);
