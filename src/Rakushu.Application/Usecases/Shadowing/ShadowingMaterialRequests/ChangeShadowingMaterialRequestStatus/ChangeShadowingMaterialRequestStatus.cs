using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.ChangeShadowingMaterialRequestStatus;

public sealed record ChangeShadowingMaterialRequestStatusCommand(
	Guid RequestId,
	ShadowingMaterialRequestStatus Status,
	string? RejectReason) : IRequest<Result>;

internal sealed class ChangeShadowingMaterialRequestStatusValidator
	: AbstractValidator<ChangeShadowingMaterialRequestStatusCommand>
{
	public ChangeShadowingMaterialRequestStatusValidator()
	{
		RuleFor(x => x.RequestId).NotEmpty().WithMessage("RequestId is required");
		RuleFor(x => x.Status)
			.Must(s => s is ShadowingMaterialRequestStatus.Processing or ShadowingMaterialRequestStatus.Rejected)
			.WithMessage("Status must be Processing (approve) or Rejected. Completed is set automatically when the material is Ready");
		RuleFor(x => x.RejectReason)
			.NotEmpty().When(x => x.Status == ShadowingMaterialRequestStatus.Rejected)
			.WithMessage("RejectReason is required when rejecting a request");
		RuleFor(x => x.RejectReason).MaximumLength(1000);
	}
}

internal sealed class ChangeShadowingMaterialRequestStatusHandler(
	IShadowingMaterialRequestRepository repository,
	IShadowingMaterialRepository materialRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<ChangeShadowingMaterialRequestStatusCommand, Result>
{
	public async Task<Result> Handle(ChangeShadowingMaterialRequestStatusCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var entity = await repository.GetByIdAsync(ShadowingMaterialRequestId.From(request.RequestId), cancellationToken);
			if (entity is null)
				return Result.Failure(ShadowingMaterialRequestErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var changed = entity.ChangeStatus(request.Status, request.RejectReason, now);
			if (changed.IsFailure)
				return changed;

			if (request.Status == ShadowingMaterialRequestStatus.Processing)
			{
				var version = await materialRepository.GetNextVersionAsync(entity.VideoId, cancellationToken);
				var material = ShadowingMaterial.Create(
					MaterialType.RequestedByLearner, entity.Id, entity.VideoId, version, now);
				if (material.IsFailure)
					return Result.Failure(material.Error);

				materialRepository.Add(material.Value);
			}

			repository.Update(entity);
			return Result.Success();
		}, cancellationToken);
	}
}
