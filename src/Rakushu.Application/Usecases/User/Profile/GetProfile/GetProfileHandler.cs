using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.User.User.GetUserById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.User.Profile.GetProfile;

internal sealed class GetProfileHandler : IRequestHandler<GetProfileQuery, Result<UserDetailDto>>
{
	// USER CONTEXT
	private readonly ICurrentUserContext _currentUserContext;

	// DAOs
	private readonly IUserQuery _userQuery;

	// SERVICES
	private readonly IStorageService _storageService;

	public GetProfileHandler(
		ICurrentUserContext currentUserContext,
		IUserQuery userQuery,
		IStorageService storageService)
	{
		_currentUserContext = currentUserContext;
		_userQuery = userQuery;
		_storageService = storageService;
	}

	public async Task<Result<UserDetailDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;

		if(userId == null)
		{
			return Result.Failure<UserDetailDto>(UserErrors.NotFound);
		}

		var user = await _userQuery.GetByIdAsync(userId, cancellationToken);

		if (user == null)
		{
			return Result.Failure<UserDetailDto>(UserErrors.NotFound);
		}

		// Process avatar URL if profile exists
		if (user.Profile?.Avatar != null)
		{
			var avatarUrl = await _storageService.CreatePresignedReadUrlAsync(user.Profile.Avatar, cancellationToken);

			var updatedProfile = user.Profile with { Avatar = avatarUrl };

			var updatedUser = user with { Profile = updatedProfile };

			return Result.Success(updatedUser);
		}

		return Result.Success(user);
	}
}
