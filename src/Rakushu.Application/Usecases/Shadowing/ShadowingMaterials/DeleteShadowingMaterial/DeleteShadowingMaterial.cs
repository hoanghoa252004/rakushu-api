using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.DeleteShadowingMaterial;

public sealed record DeleteShadowingMaterialCommand(Guid MaterialId) : IRequest<Result>;

internal sealed class DeleteShadowingMaterialValidator : AbstractValidator<DeleteShadowingMaterialCommand>
{
	public DeleteShadowingMaterialValidator()
	{
		RuleFor(x => x.MaterialId).NotEmpty().WithMessage("MaterialId is required");
	}
}

internal sealed class DeleteShadowingMaterialHandler(
	IShadowingMaterialRepository repository,
	IVideoRepository videoRepository,
	ICurrentUserContext currentUserContext,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteShadowingMaterialCommand, Result>
{
	public async Task<Result> Handle(DeleteShadowingMaterialCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var material = await repository.GetByIdAsync(ShadowingMaterialId.From(request.MaterialId), cancellationToken);
			if (material is null)
				return Result.Failure(ShadowingMaterialErrors.NotFound);

			if (currentUserContext.Role == RoleCodes.Learner)
			{
				if (currentUserContext.UserId is null || material.CreatedById != currentUserContext.UserId)
					return Result.Failure(ShadowingMaterialErrors.LearnerNotAllowed);
			}
			else if (currentUserContext.Role != RoleCodes.LinguisticCurator)
				return Result.Failure(ShadowingMaterialErrors.LearnerNotAllowed);

			if (await repository.HasSessionsAsync(material.Id, cancellationToken))
				return Result.Failure(ShadowingMaterialErrors.HasSessions);

			repository.Delete(material);
			return Result.Success();
		}, cancellationToken);
	}
}
