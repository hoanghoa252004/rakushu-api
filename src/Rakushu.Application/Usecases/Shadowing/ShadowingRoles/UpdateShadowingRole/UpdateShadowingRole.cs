using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoles.UpdateShadowingRole;

public sealed record UpdateShadowingRoleCommand(Guid RoleId, Guid SpeakerId, int SortOrder) : IRequest<Result>;

internal sealed class UpdateShadowingRoleValidator : AbstractValidator<UpdateShadowingRoleCommand>
{
	public UpdateShadowingRoleValidator()
	{
		RuleFor(x => x.RoleId).NotEmpty();
		RuleFor(x => x.SpeakerId).NotEmpty();
		RuleFor(x => x.SortOrder).GreaterThan(0);
	}
}

internal sealed class UpdateShadowingRoleHandler(
	IShadowingRoleRepository repository,
	IShadowingMaterialRepository materialRepository,
	ISpeakerRepository speakerRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<UpdateShadowingRoleCommand, Result>
{
	public async Task<Result> Handle(UpdateShadowingRoleCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var roleId = ShadowingRoleId.From(request.RoleId);
			var role = await repository.GetByIdAsync(roleId, cancellationToken);
			if (role is null)
				return Result.Failure(ShadowingRoleErrors.NotFound);

			var material = await materialRepository.GetByIdAsync(role.ShadowingMaterialId, cancellationToken);
			if (material is null)
				return Result.Failure(ShadowingMaterialErrors.NotFound);
			if (material.Status != ShadowingMaterialStatus.Reviewing)
				return Result.Failure(ShadowingMaterialErrors.NotEditable);

			var speakerId = SpeakerId.From(request.SpeakerId);
			if (await speakerRepository.GetByIdAsync(speakerId, cancellationToken) is null)
				return Result.Failure(SpeakerErrors.NotFound);

			if (await repository.SpeakerExistsAsync(role.ShadowingMaterialId, speakerId, roleId, cancellationToken))
				return Result.Failure(ShadowingRoleErrors.SpeakerAlreadyExists);
			if (!await repository.VideoHasSpeakerAsync(material.VideoId, speakerId, cancellationToken))
				return Result.Failure(ShadowingRoleErrors.SpeakerNotInTranscript);
			if (await repository.SortOrderExistsAsync(role.ShadowingMaterialId, request.SortOrder, roleId, cancellationToken))
				return Result.Failure(ShadowingRoleErrors.SortOrderAlreadyExists);

			var updated = role.Update(speakerId, request.SortOrder);
			if (updated.IsFailure)
				return updated;

			repository.Update(role);
			return Result.Success();
		}, cancellationToken);
	}
}
