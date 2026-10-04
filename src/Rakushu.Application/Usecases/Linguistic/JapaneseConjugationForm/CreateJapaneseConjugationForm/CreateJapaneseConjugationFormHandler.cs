using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.CreateJapaneseConjugationForm;

internal sealed class CreateJapaneseConjugationFormHandler : IRequestHandler<CreateJapaneseConjugationFormCommand, Result<Guid>>
{
	private readonly IJapaneseConjugationFormRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateJapaneseConjugationFormHandler(IJapaneseConjugationFormRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateJapaneseConjugationFormCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var form = Domain.Entities.Linguistic.JapaneseConjugationForm.JapaneseConjugationForm.Create(
				JapaneseConjugationFormId.Create(),
				request.code,
				request.name,
				request.vietnameseName,
				request.description);

			_repository.Add(form);
			return Result.Success(form.Id.Value);
		}, cancellationToken);
	}
}