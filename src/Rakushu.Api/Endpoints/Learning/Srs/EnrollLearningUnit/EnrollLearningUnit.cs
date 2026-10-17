using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.EnrollLearningUnit;

namespace Rakushu.Api.Endpoints.Learning.Srs.EnrollLearningUnit;

internal sealed class EnrollLearningUnit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapSrsEndpoints()
            .MapPost("/notes", async ([FromBody] EnrollLearningUnitRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new EnrollLearningUnitCommand(dto.LearningUnitId), cancellationToken);
                return result.MatchOk();
            })
            .WithName("EnrollLearningUnitInSrs")
            .WithSummary("Enroll a learning unit in SRS")
            .WithDescription("Creates a note, card and initial scheduling state for the current user.")
            .RequireAuthorization()
            .Produces<EnrollLearningUnitResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

internal sealed record EnrollLearningUnitRequestDto(Guid LearningUnitId);
