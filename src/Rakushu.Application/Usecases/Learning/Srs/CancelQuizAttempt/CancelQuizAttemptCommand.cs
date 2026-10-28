using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.CancelQuizAttempt;

public sealed record CancelQuizAttemptCommand(Guid AttemptId) : IRequest<Result>;
