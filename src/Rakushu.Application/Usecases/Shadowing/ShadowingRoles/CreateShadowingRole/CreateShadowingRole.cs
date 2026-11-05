using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoles.CreateShadowingRole;

public sealed record CreateShadowingRoleCommand(Guid MaterialId, Guid SpeakerId, int SortOrder) : IRequest<Result<Guid>>;

internal sealed class CreateShadowingRoleValidator : AbstractValidator<CreateShadowingRoleCommand>
{
	public CreateShadowingRoleValidator()
	{
		RuleFor(x => x.MaterialId).NotEmpty();
		RuleFor(x => x.SpeakerId).NotEmpty();
		RuleFor(x => x.SortOrder).GreaterThan(0);
	}
}

internal sealed class CreateShadowingRoleHandler(
	IShadowingRoleRepository repository,
	IShadowingMaterialRepository materialRepository,
	ISpeakerRepository speakerRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateShadowingRoleCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateShadowingRoleCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var materialId = ShadowingMaterialId.From(request.MaterialId);
			var material = await materialRepository.GetByIdAsync(materialId, cancellationToken);
			if (material is null)
				return Result.Failure<Guid>(ShadowingMaterialErrors.NotFound);
			if (material.Status != ShadowingMaterialStatus.Reviewing)
				return Result.Failure<Guid>(ShadowingMaterialErrors.NotEditable);

			var speakerId = SpeakerId.From(request.SpeakerId);
			if (await speakerRepository.GetByIdAsync(speakerId, cancellationToken) is null)
				return Result.Failure<Guid>(SpeakerErrors.NotFound);

			if (await repository.SpeakerExistsAsync(materialId, speakerId, null, cancellationToken))
				return Result.Failure<Guid>(ShadowingRoleErrors.SpeakerAlreadyExists);
			if (await repository.SortOrderExistsAsync(materialId, request.SortOrder, null, cancellationToken))
				return Result.Failure<Guid>(ShadowingRoleErrors.SortOrderAlreadyExists);

			var created = ShadowingRole.Create(materialId, speakerId, request.SortOrder);
			if (created.IsFailure)
				return Result.Failure<Guid>(created.Error);

			repository.Add(created.Value);
			return Result.Success(created.Value.Id.Value);
		}, cancellationToken);
	}
}
