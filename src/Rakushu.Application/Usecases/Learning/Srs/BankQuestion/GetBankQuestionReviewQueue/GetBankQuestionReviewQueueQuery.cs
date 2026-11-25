using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestionReviewQueue;

public sealed record GetBankQuestionReviewQueueQuery(
	QuestionStatus? Status = QuestionStatus.Draft,
	QuestionSourceType? SourceType = null,
	string? Level = null,
	string? QuestionTypeCode = null,
	string? MediaType = null,
	string? Search = null,
	int Page = 1,
	int PageSize = 20) : IRequest<Result<BankQuestionReviewQueueResponseDto>>;
