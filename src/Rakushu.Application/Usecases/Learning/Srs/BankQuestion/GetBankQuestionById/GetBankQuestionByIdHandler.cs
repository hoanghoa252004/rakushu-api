using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestionById;

internal sealed class GetBankQuestionByIdHandler : IRequestHandler<GetBankQuestionByIdQuery, Result<BankQuestionDetailDto>>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;

	public GetBankQuestionByIdHandler(IBankQuestionRepository bankQuestionRepository)
	{
		_bankQuestionRepository = bankQuestionRepository;
	}

	public async Task<Result<BankQuestionDetailDto>> Handle(
		GetBankQuestionByIdQuery request,
		CancellationToken cancellationToken)
	{
		var questionId = BankQuestionId.From(request.Id);
		var bq = await _bankQuestionRepository.GetByIdWithDetailsAsync(questionId, cancellationToken);
		if (bq is null)
			return Result.Failure<BankQuestionDetailDto>(BankQuestionErrors.NotFound);

		var parsedChoices = QuizOptionParser.Parse(bq.ChoicesJson)
			.Select(opt => new BankQuestionChoiceDto(opt.Id, opt.Text))
			.ToList();

		var dto = new BankQuestionDetailDto(
			bq.Id.Value,
			bq.KnowledgeMeaningId.Value,
			bq.KnowledgeMeaning?.KnowledgeUnit?.Expression,
			bq.KnowledgeMeaning?.Meaning,
			bq.QuestionTypeId.Value,
			bq.QuestionType?.Code,
			bq.Prompt,
			parsedChoices,
			bq.CorrectChoiceId,
			bq.Explanation,
			bq.Difficulty,
			bq.IsActive,
			bq.CreatedAt,
			bq.UpdatedAt);

		return Result.Success(dto);
	}
}
