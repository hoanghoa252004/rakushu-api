using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestions;

internal sealed class GetBankQuestionsHandler : IRequestHandler<GetBankQuestionsQuery, Result<IReadOnlyList<BankQuestionSummaryDto>>>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;

	public GetBankQuestionsHandler(IBankQuestionRepository bankQuestionRepository)
	{
		_bankQuestionRepository = bankQuestionRepository;
	}

	public async Task<Result<IReadOnlyList<BankQuestionSummaryDto>>> Handle(
		GetBankQuestionsQuery request,
		CancellationToken cancellationToken)
	{
		KnowledgeMeaningId? meaningId = request.MeaningId.HasValue && request.MeaningId.Value != Guid.Empty
			? KnowledgeMeaningId.From(request.MeaningId.Value)
			: null;

		int page = Math.Max(1, request.Page);
		int pageSize = Math.Clamp(request.PageSize, 1, 100);

		var (items, totalCount) = await _bankQuestionRepository.GetQuestionsAsync(
			meaningId,
			request.QuestionTypeId,
			request.Difficulty,
			request.IsActive,
			page,
			pageSize,
			cancellationToken);

		var dtos = items.Select(bq => new BankQuestionSummaryDto(
			bq.Id.Value,
			bq.KnowledgeMeaningId.Value,
			bq.KnowledgeMeaning?.KnowledgeUnit?.Expression,
			bq.KnowledgeMeaning?.Meaning,
			bq.QuestionTypeId.Value,
			bq.QuestionType?.Code,
			bq.Prompt,
			bq.Difficulty,
			bq.IsActive,
			bq.CreatedAt)).ToList();

		return Result.Success<IReadOnlyList<BankQuestionSummaryDto>>(dtos);
	}
}
