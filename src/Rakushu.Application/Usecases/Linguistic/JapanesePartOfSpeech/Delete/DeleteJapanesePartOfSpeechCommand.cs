using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Delete;

public sealed record DeleteJapanesePartOfSpeechCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteJapanesePartOfSpeechValidator : AbstractValidator<DeleteJapanesePartOfSpeechCommand>
{
	public DeleteJapanesePartOfSpeechValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("ID is required");
	}
}

internal sealed class DeleteJapanesePartOfSpeechHandler : IRequestHandler<DeleteJapanesePartOfSpeechCommand, Result>
{
	private readonly IJapanesePartOfSpeechRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteJapanesePartOfSpeechHandler(IJapanesePartOfSpeechRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteJapanesePartOfSpeechCommand command, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var id = JapanesePartOfSpeechId.From(command.Id);
			var entity = await _repository.GetByIdAsync(id, cancellationToken);

			if (entity is null)
				return Result.Failure(LinguisticErrors.NotFound);

			_repository.Delete(entity);
			return Result.Success();
		});
	}
}
