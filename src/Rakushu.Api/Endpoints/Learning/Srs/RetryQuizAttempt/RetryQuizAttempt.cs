using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

namespace Rakushu.Api.Endpoints.Learning.Srs.RetryQuizAttempt;

internal sealed class RetryQuizAttempt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapSrsEndpoints()
            .MapPost(
                "/quiz-attempts/{attemptId:guid}/retry",
                async (
                    Guid attemptId,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                        new RetryQuizAttemptCommand(attemptId),
                        cancellationToken);

                    return result.MatchOk();
                })
            .WithName("RetrySrsQuizAttempt")
            .WithSummary("Retry a completed quiz with the same questions")
            .WithDescription(
                "Creates a Practice attempt using the original question snapshots. " +
                "Retrying does not update SM-2.")
            .RequireAuthorization()
            .Produces<QuizAttemptStartedResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
