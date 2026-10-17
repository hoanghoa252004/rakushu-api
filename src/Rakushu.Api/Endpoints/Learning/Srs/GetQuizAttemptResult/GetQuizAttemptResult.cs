using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.GetQuizAttemptResult;
using Rakushu.Application.Usecases.Learning.Srs.Common;

namespace Rakushu.Api.Endpoints.Learning.Srs.GetQuizAttemptResult;

internal sealed class GetQuizAttemptResult : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapSrsEndpoints()
            .MapGet(
                "/quiz-attempts/{attemptId:guid}/result",
                async (
                    Guid attemptId,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                        new GetQuizAttemptResultQuery(attemptId),
                        cancellationToken);

                    return result.MatchOk();
                })
            .WithName("GetSrsQuizAttemptResult")
            .WithSummary("Retrieve a completed quiz result")
            .RequireAuthorization()
            .Produces<QuizAttemptResultResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
