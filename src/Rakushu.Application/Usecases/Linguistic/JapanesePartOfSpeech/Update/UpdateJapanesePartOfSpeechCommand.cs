using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Update;

public sealed record UpdateJapanesePartOfSpeechCommand(
	Guid Id,
	string Name,
	string JapaneseName,
	string? Description) : IRequest<Result>;

public sealed class UpdateJapanesePartOfSpeechValidator : AbstractValidator<UpdateJapanesePartOfSpeechCommand>
{
	public UpdateJapanesePartOfSpeechValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("ID is required");

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("Name is required")
			.MaximumLength(200)
			.WithMessage("Name must not exceed 200 characters");

		RuleFor(x => x.JapaneseName)
			.NotEmpty()
			.WithMessage("Japanese name is required")
			.MaximumLength(200)
			.WithMessage("Japanese name must not exceed 200 characters");

		RuleFor(x => x.Description)
			.MaximumLength(500)
			.WithMessage("Description must not exceed 500 characters")
			.When(x => x.Description != null);
	}
}

internal sealed class UpdateJapanesePartOfSpeechHandler : IRequestHandler<UpdateJapanesePartOfSpeechCommand, Result>
{
	private readonly IJapanesePartOfSpeechRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateJapanesePartOfSpeechHandler(IJapanesePartOfSpeechRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateJapanesePartOfSpeechCommand command, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var id = JapanesePartOfSpeechId.From(command.Id);
			var entity = await _repository.GetByIdAsync(id, cancellationToken);

			if (entity is null)
				return Result.Failure(LinguisticErrors.NotFound);

			entity.Update(command.Name, command.JapaneseName, command.Description);
			_repository.Update(entity);
			return Result.Success();
		});
	}
}
