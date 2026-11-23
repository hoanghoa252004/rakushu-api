using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.QuestionType;

namespace Rakushu.Application.Usecases.Learning.Srs.QuestionType.GetQuestionTypes;

internal sealed class GetQuestionTypesHandler : IRequestHandler<GetQuestionTypesQuery, Result<IReadOnlyList<QuestionTypeDto>>>
{
	private readonly IQuestionTypeRepository _questionTypeRepository;

	public GetQuestionTypesHandler(IQuestionTypeRepository questionTypeRepository)
	{
		_questionTypeRepository = questionTypeRepository;
	}

	public async Task<Result<IReadOnlyList<QuestionTypeDto>>> Handle(
		GetQuestionTypesQuery request,
		CancellationToken cancellationToken)
	{
		var types = await _questionTypeRepository.GetActiveQuestionTypesAsync(cancellationToken);

		var dtos = types
			.OrderBy(qt => qt.Code)
			.Select(qt => new QuestionTypeDto(
				qt.Id.Value,
				qt.Code,
				qt.Name,
				qt.Description,
				qt.ExpectedDurationSeconds,
				qt.IsActive))
			.ToList();

		return Result.Success<IReadOnlyList<QuestionTypeDto>>(dtos);
	}
}
