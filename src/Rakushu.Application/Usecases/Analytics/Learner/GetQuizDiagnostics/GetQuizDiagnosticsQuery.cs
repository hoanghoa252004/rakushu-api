using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Learner.GetQuizDiagnostics;

public sealed record GetQuizDiagnosticsQuery(
	int? Days = null,
	string? QuestionTypeCode = null) : IRequest<Result<QuestionTypeDetailResponseDto>>;
