using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using JapaneseConjugationFormEntity = Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm.JapaneseConjugationForm;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Create;

internal sealed class CreateJapaneseConjugationFormHandler : IRequestHandler<CreateJapaneseConjugationFormCommand, Result<Guid>>
{
	private readonly IJapaneseConjugationFormRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateJapaneseConjugationFormHandler(IJapaneseConjugationFormRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateJapaneseConjugationFormCommand command, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var id = JapaneseConjugationFormId.Create();
			var entity = JapaneseConjugationFormEntity.Create(
				id,
				command.Code,
				command.Name,
				command.JapaneseName,
				command.Description);

			_repository.Add(entity);
			return Result.Success(id.Value);
		});
	}
}
