using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestionById;

internal sealed class GetBankQuestionByIdHandler : IRequestHandler<GetBankQuestionByIdQuery, Result<BankQuestionDetailDto>>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly IStorageService _storageService;

	public GetBankQuestionByIdHandler(
		IBankQuestionRepository bankQuestionRepository,
		IStorageService storageService)
	{
		_bankQuestionRepository = bankQuestionRepository;
		_storageService = storageService;
	}

	public async Task<Result<BankQuestionDetailDto>> Handle(
		GetBankQuestionByIdQuery request,
		CancellationToken cancellationToken)
	{
		var questionId = BankQuestionId.From(request.Id);
		var bq = await _bankQuestionRepository.GetByIdWithDetailsAsync(questionId, cancellationToken);
		if (bq is null)
			return Result.Failure<BankQuestionDetailDto>(BankQuestionErrors.NotFound);

		var parsedAnswers = QuizOptionParser.Parse(bq.Answers)
			.Select(opt => new QuestionAnswerDto(opt.Id, opt.Text))
			.ToList();

		string? resolvedMediaUrl = bq.MediaUrl;
		if (!string.IsNullOrWhiteSpace(resolvedMediaUrl) &&
		    !resolvedMediaUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
		    !resolvedMediaUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
		{
			try
			{
				resolvedMediaUrl = await _storageService.CreatePresignedReadUrlAsync(resolvedMediaUrl, cancellationToken);
			}
			catch
			{
				resolvedMediaUrl = bq.MediaUrl;
			}
		}

		var parsedMetadata = QuestionMediaMetadataParser.Parse(bq.MediaMetadata);

		var dto = new BankQuestionDetailDto(
			bq.Id.Value,
			bq.KnowledgeMeaningId.Value,
			bq.KnowledgeMeaning?.KnowledgeUnit?.Expression,
			bq.KnowledgeMeaning?.Meaning,
			bq.QuestionTypeId.Value,
			bq.QuestionType?.Code,
			bq.Question,
			parsedAnswers,
			bq.CorrectAnswer,
			bq.Explanation,
			bq.IsActive,
			bq.CreatedAt,
			bq.UpdatedAt,
			bq.MediaType,
			resolvedMediaUrl,
			parsedMetadata);

		return Result.Success(dto);
	}
}
