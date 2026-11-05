using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.DeleteShadowingMaterialRequest;

public sealed record DeleteShadowingMaterialRequestCommand(Guid RequestId) : IRequest<Result>;

internal sealed class DeleteShadowingMaterialRequestValidator : AbstractValidator<DeleteShadowingMaterialRequestCommand>
{
	public DeleteShadowingMaterialRequestValidator()
	{
		RuleFor(x => x.RequestId).NotEmpty().WithMessage("RequestId is required");
	}
}

internal sealed class DeleteShadowingMaterialRequestHandler(
	IShadowingMaterialRequestRepository repository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteShadowingMaterialRequestCommand, Result>
{
	public async Task<Result> Handle(DeleteShadowingMaterialRequestCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var entity = await repository.GetByIdAsync(ShadowingMaterialRequestId.From(request.RequestId), cancellationToken);
			if (entity is null)
				return Result.Failure(ShadowingMaterialRequestErrors.NotFound);

			if (currentUser.Role != RoleCodes.LinguisticCurator && entity.LearnerId != currentUser.UserId)
				return Result.Failure(ShadowingMaterialRequestErrors.Forbidden);

			if (entity.Status != ShadowingMaterialRequestStatus.Pending)
				return Result.Failure(ShadowingMaterialRequestErrors.CannotModify);

			repository.Delete(entity);
			return Result.Success();
		}, cancellationToken);
	}
}
