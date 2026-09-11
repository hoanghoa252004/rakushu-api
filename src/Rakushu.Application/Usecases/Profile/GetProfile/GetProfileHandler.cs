using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Profile.GetProfile;

internal sealed class GetProfileHandler : IRequestHandler<GetProfileQuery, Result<UserDto>>
{
	// USER CONTEXT
	private readonly ICurrentUserContext _currentUserContext;

	// DAOs
	private readonly IUserQuery _userQuery;

	public GetProfileHandler(
		ICurrentUserContext currentUserContext,
		IUserQuery userQuery)
	{
		_currentUserContext = currentUserContext;
		_userQuery = userQuery;
	}

	public async Task<Result<UserDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;

		var profile = await _userQuery.GetByIdAsync(userId, cancellationToken);

		if (profile == null)
		{
			return Result.Failure<UserDto>(UserError.NotFound);
		}

		// Concat Resource URL with AvatarUrl if AvatarUrl is not null
		// TODO: Implement this logic in the future

		return Result.Success(profile);
	}
}
