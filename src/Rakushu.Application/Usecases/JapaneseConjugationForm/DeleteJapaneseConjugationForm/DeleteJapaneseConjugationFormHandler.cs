using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.JapaneseConjugationForm.DeleteJapaneseConjugationForm;

internal sealed class DeleteJapaneseConjugationFormHandler : IRequestHandler<DeleteJapaneseConjugationFormCommand, Result>
{
	private readonly IJapaneseConjugationFormRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteJapaneseConjugationFormHandler(IJapaneseConjugationFormRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteJapaneseConjugationFormCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var form = await _repository.GetByIdAsync(JapaneseConjugationFormId.From(request.JapaneseConjugationFormId), cancellationToken);
			if (form is null)
				return Result.Failure(Error.NotFound("JAPANESE_CONJUGATION_FORM.NOT_FOUND", "Japanese conjugation form not found."));

			_repository.Delete(form);
			return Result.Success();
		}, cancellationToken);
	}
}