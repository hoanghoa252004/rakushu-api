using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Curator.GetCuratorHighErrorQuestions;

public sealed record GetCuratorHighErrorQuestionsQuery(
	int Days = 30,
	string? TimeZone = null,
	string? Level = null,
	string? QuestionTypeCode = null,
	string? Search = null,
	int MinAnswers = 1,
	string? SortBy = null,
	int Page = 1,
	int PageSize = 20) : IRequest<Result<CuratorHighErrorQuestionsResponseDto>>;
