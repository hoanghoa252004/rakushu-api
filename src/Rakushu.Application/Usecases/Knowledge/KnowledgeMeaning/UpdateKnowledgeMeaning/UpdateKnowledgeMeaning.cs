using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.UpdateKnowledgeMeaning;

public sealed record UpdateKnowledgeMeaningCommand(
	Guid KnowledgeMeaningId,
	int SortOrder,
	string Meaning,
	string JapaneseMeaning,
	string? Description = null
) : IRequest<Result>;

internal sealed class UpdateKnowledgeMeaningValidator : AbstractValidator<UpdateKnowledgeMeaningCommand>
{
	public UpdateKnowledgeMeaningValidator()
	{
		RuleFor(x => x.KnowledgeMeaningId).NotEmpty().WithMessage("Knowledge meaning ID is required");

		RuleFor(x => x.SortOrder).GreaterThan(0).WithMessage("Sort order must be greater than 0");

		RuleFor(x => x.Meaning)
			.NotEmpty().WithMessage("Meaning is required")
			.MaximumLength(100).WithMessage("Meaning must not exceed 100 characters");

		RuleFor(x => x.JapaneseMeaning)
			.NotEmpty().WithMessage("Japanese meaning is required")
			.MaximumLength(100).WithMessage("Japanese meaning must not exceed 100 characters");
	}
}

internal sealed class UpdateKnowledgeMeaningHandler : IRequestHandler<UpdateKnowledgeMeaningCommand, Result>
{
	private readonly IKnowledgeMeaningRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateKnowledgeMeaningHandler(IKnowledgeMeaningRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateKnowledgeMeaningCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var meaning = await _repository.GetByIdAsync(KnowledgeMeaningId.From(request.KnowledgeMeaningId), cancellationToken);

			if (meaning is null)
				return Result.Failure(KnowledgeMeaningErrors.NotFound);

			var updateResult = meaning.Update(
				request.SortOrder,
				request.Meaning,
				request.JapaneseMeaning,
				request.Description);

			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}
