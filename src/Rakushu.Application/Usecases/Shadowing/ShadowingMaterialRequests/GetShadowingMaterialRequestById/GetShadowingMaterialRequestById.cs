using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.GetShadowingMaterialRequestById;

public sealed record GetShadowingMaterialRequestByIdQuery(Guid RequestId) : IRequest<Result<ShadowingMaterialRequestDto>>;

internal sealed class GetShadowingMaterialRequestByIdValidator : AbstractValidator<GetShadowingMaterialRequestByIdQuery>
{
	public GetShadowingMaterialRequestByIdValidator()
	{
		RuleFor(x => x.RequestId).NotEmpty().WithMessage("RequestId is required");
	}
}

internal sealed class GetShadowingMaterialRequestByIdHandler(
	IShadowingMaterialRequestRepository repository,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingMaterialRequestByIdQuery, Result<ShadowingMaterialRequestDto>>
{
	public async Task<Result<ShadowingMaterialRequestDto>> Handle(
		GetShadowingMaterialRequestByIdQuery request, CancellationToken cancellationToken)
	{
		var entity = await repository.GetByIdAsync(ShadowingMaterialRequestId.From(request.RequestId), cancellationToken);
		if (entity is null)
			return Result.Failure<ShadowingMaterialRequestDto>(ShadowingMaterialRequestErrors.NotFound);

		if (currentUser.Role != RoleCodes.LinguisticCurator && entity.LearnerId != currentUser.UserId)
			return Result.Failure<ShadowingMaterialRequestDto>(ShadowingMaterialRequestErrors.Forbidden);

		return Result.Success(ShadowingMaterialRequestDto.FromEntity(entity));
	}
}
