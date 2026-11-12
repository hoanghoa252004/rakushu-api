using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingSessions.GetShadowingSession;

public sealed record GetShadowingSessionQuery(Guid SessionId) : IRequest<Result<ShadowingSessionDto>>;

internal sealed class GetShadowingSessionValidator : AbstractValidator<GetShadowingSessionQuery>
{
	public GetShadowingSessionValidator() => RuleFor(x => x.SessionId).NotEmpty();
}

internal sealed class GetShadowingSessionHandler(
	IShadowingSessionRepository repository,
	IStorageService storage,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingSessionQuery, Result<ShadowingSessionDto>>
{
	public async Task<Result<ShadowingSessionDto>> Handle(GetShadowingSessionQuery request, CancellationToken cancellationToken)
	{
		var session = await repository.GetDetailByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
		if (session is null)
			return Result.Failure<ShadowingSessionDto>(ShadowingSessionErrors.NotFound);

		var isCurator = currentUser.Role == RoleCodes.LinguisticCurator;
		var isMember = currentUser.UserId is not null && session.Members.Any(m => m.MemberId == currentUser.UserId);
		var isPublic = session.Visibility == ShadowingSessionVisibility.Public;
		if (!isCurator && !isMember && !isPublic)
			return Result.Failure<ShadowingSessionDto>(ShadowingSessionErrors.NotPublic);

		return Result.Success(ShadowingSessionDto.FromEntity(
			session, await SessionAvatarResolver.ResolveAsync([session], storage, cancellationToken)));
	}
}
