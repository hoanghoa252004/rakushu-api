using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

namespace Rakushu.Api.Endpoints.Learning.Srs.CreateQuizAttempt;

internal sealed class CreateQuizAttempt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapSrsEndpoints()
            .MapPost("/quiz-attempts", async (
                [FromBody] CreateQuizAttemptRequestDto dto,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateQuizAttemptCommand(
                    dto.CardIds,
                    dto.AttemptMode ?? "SrsReview");

                var result = await sender.Send(command, cancellationToken);
                return result.MatchOk();
            })
            .WithName("CreateSrsQuizAttempt")
            .WithSummary("Start a fixed-template SRS quiz from selected cards")
            .WithDescription(
                "Provide the SRS card IDs selected by the learner. SrsReview accepts only due cards; " +
                "Practice accepts enrolled cards and does not update SM-2 on submission.")
            .RequireAuthorization()
            .Produces<QuizAttemptStartedResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

internal sealed record CreateQuizAttemptRequestDto(
    IReadOnlyList<Guid>? CardIds,
    string? AttemptMode = null);
