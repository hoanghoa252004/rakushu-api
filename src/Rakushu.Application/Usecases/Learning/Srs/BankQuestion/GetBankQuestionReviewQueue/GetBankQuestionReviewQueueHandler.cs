using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestionReviewQueue;

internal sealed class GetBankQuestionReviewQueueHandler
	: IRequestHandler<GetBankQuestionReviewQueueQuery, Result<BankQuestionReviewQueueResponseDto>>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;

	public GetBankQuestionReviewQueueHandler(IBankQuestionRepository bankQuestionRepository)
	{
		_bankQuestionRepository = bankQuestionRepository;
	}

	public async Task<Result<BankQuestionReviewQueueResponseDto>> Handle(
		GetBankQuestionReviewQueueQuery request,
		CancellationToken cancellationToken)
	{
		int page = Math.Max(1, request.Page);
		int pageSize = Math.Clamp(request.PageSize, 1, 100);

		var (items, totalCount) = await _bankQuestionRepository.GetReviewQueueAsync(
			request.Status,
			request.Level,
			request.QuestionTypeCode,
			request.MediaType,
			request.Search,
			page,
			pageSize,
			cancellationToken);

		int totalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 1;
		int safePage = Math.Clamp(page, 1, totalPages);

		var dtos = items.Select(bq => new BankQuestionReviewItemDto(
			bq.Id.Value,
			bq.KnowledgeMeaningId.Value,
			bq.KnowledgeMeaning?.KnowledgeUnit?.Expression,
			bq.KnowledgeMeaning?.KnowledgeUnit?.Reading,
			bq.KnowledgeMeaning?.Meaning,
			bq.KnowledgeMeaning?.KnowledgeUnit?.ProficiencyLevel?.Code?.Trim().ToUpperInvariant(),
			bq.QuestionTypeId.Value,
			bq.QuestionType?.Code,
			bq.QuestionType?.Name,
			bq.Question,
			bq.IsActive,
			bq.Status,
			string.IsNullOrWhiteSpace(bq.MediaType) ? "NONE" : bq.MediaType.Trim().ToUpperInvariant(),
			bq.CreatedAt)).ToList();

		var response = new BankQuestionReviewQueueResponseDto(
			totalCount,
			safePage,
			pageSize,
			totalPages,
			dtos);

		return Result.Success(response);
	}
}
