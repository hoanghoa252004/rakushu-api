using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingSessions.GetShadowingSessionMaterial;

public sealed record GetShadowingSessionMaterialQuery(
	Guid SessionId,
	int LinePageNumber = 1,
	int LinePageSize = 20) : IRequest<Result<ShadowingMaterials.ShadowingMaterialDto>>;

internal sealed class GetShadowingSessionMaterialValidator : AbstractValidator<GetShadowingSessionMaterialQuery>
{
	public GetShadowingSessionMaterialValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.LinePageNumber).GreaterThan(0);
		RuleFor(x => x.LinePageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetShadowingSessionMaterialHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingMaterialRepository materialRepository,
	IShadowingRoleRepository roleRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IStorageService storageService,
	Rakushu.Application.Abstractions.Infrastructure.Authentication.ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingSessionMaterialQuery, Result<ShadowingMaterials.ShadowingMaterialDto>>
{
	public async Task<Result<ShadowingMaterials.ShadowingMaterialDto>> Handle(
		GetShadowingSessionMaterialQuery request, CancellationToken cancellationToken)
	{
		var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
		if (session is null)
			return Result.Failure<ShadowingMaterials.ShadowingMaterialDto>(ShadowingSessionErrors.NotFound);

		if (currentUser.UserId is null || !session.Members.Any(m => m.MemberId == currentUser.UserId))
			return Result.Failure<ShadowingMaterials.ShadowingMaterialDto>(ShadowingSessionErrors.NotMember);

		var material = await materialRepository.GetByIdAsync(session.ShadowingMaterialId, cancellationToken);
		if (material is null)
			return Result.Failure<ShadowingMaterials.ShadowingMaterialDto>(ShadowingMaterialErrors.NotFound);

		return Result.Success(await ShadowingMaterials.ShadowingMaterialDto.FromEntityWithDetailsAsync(
			material, roleRepository, roleLineRepository, storageService,
			request.LinePageNumber, request.LinePageSize, cancellationToken));
	}
}
