using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttempt;

public sealed record GetQuizAttemptQuery(Guid AttemptId) : IRequest<Result<QuizAttemptDetailDto>>;
