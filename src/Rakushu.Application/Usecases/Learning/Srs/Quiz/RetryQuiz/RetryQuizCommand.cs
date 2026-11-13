using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Quiz.RetryQuiz;

public sealed record RetryQuizCommand(Guid QuizId) : IRequest<Result<Guid>>;
