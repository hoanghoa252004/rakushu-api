using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

namespace Rakushu.Api.Endpoints.Learning.Srs.SubmitQuizAttempt;

internal sealed class SubmitQuizAttempt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapSrsEndpoints()
            .MapPost(
                "/quiz-attempts/{attemptId:guid}/submit",
                async (
                    Guid attemptId,
                    [FromBody] SubmitQuizAttemptRequestDto dto,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var answers = dto.Answers
                        .Select(answer => new QuizAnswerSubmission(
                            answer.ItemId,
                            answer.SelectedOptionId))
                        .ToList();

                    var command = new SubmitQuizAttemptCommand(attemptId, answers);
                    var result = await sender.Send(command, cancellationToken);

                    return result.MatchOk();
                })
            .WithName("SubmitSrsQuizAttempt")
            .WithSummary("Submit all answers and get quiz feedback")
            .WithDescription(
                "Submits every answer in one request, grades the attempt, and updates SM-2 " +
                "once for each card in SrsReview mode.")
            .RequireAuthorization()
            .Produces<QuizAttemptResultResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

internal sealed record SubmitQuizAnswerDto(Guid ItemId, string SelectedOptionId);

internal sealed record SubmitQuizAttemptRequestDto(
    IReadOnlyList<SubmitQuizAnswerDto> Answers);
