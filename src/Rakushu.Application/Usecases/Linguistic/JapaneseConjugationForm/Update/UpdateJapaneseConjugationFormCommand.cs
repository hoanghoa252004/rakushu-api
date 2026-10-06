using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using JapaneseConjugationFormEntity = Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm.JapaneseConjugationForm;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;
using Rakushu.Domain.Entities.LinguisticMetadata;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Update;

public sealed record UpdateJapaneseConjugationFormCommand(
	Guid Id,
	string Name,
	string JapaneseName,
	string? Description) : IRequest<Result>;

internal sealed class UpdateJapaneseConjugationFormHandler : IRequestHandler<UpdateJapaneseConjugationFormCommand, Result>
{
	private readonly IJapaneseConjugationFormRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateJapaneseConjugationFormHandler(IJapaneseConjugationFormRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateJapaneseConjugationFormCommand command, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var id = JapaneseConjugationFormId.From(command.Id);
			var entity = await _repository.GetByIdAsync(id, cancellationToken);

			if (entity is null)
				return Result.Failure(LinguisticErrors.NotFound);

			entity.Update(command.Name, command.JapaneseName, command.Description);
			_repository.Update(entity);
			return Result.Success();
		});
	}
}
