using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.JapaneseConjugationForm.UpdateJapaneseConjugationForm;

internal sealed class UpdateJapaneseConjugationFormHandler : IRequestHandler<UpdateJapaneseConjugationFormCommand, Result>
{
	private readonly IJapaneseConjugationFormRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateJapaneseConjugationFormHandler(IJapaneseConjugationFormRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateJapaneseConjugationFormCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var form = await _repository.GetByIdAsync(JapaneseConjugationFormId.From(request.JapaneseConjugationFormId), cancellationToken);
			if (form is null)
				return Result.Failure(Error.NotFound("JAPANESE_CONJUGATION_FORM.NOT_FOUND", "Japanese conjugation form not found."));

			form.Update(request.name, request.vietnameseName, request.description);
			return Result.Success();
		}, cancellationToken);
	}
}