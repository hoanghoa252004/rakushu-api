using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.User.User.GetUserById;

internal sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailDto>>
{
	// DAOs
	private readonly IUserQuery _userQuery;

	// SERVICES
	private readonly IStorageService _storageService;

	public GetUserByIdHandler(
		IUserQuery userQuery,
		IStorageService storageService
		)
	{
		_userQuery = userQuery;
		_storageService = storageService;
	}

	public async Task<Result<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var userId = UserId.From(request.UserId);

		var user = await _userQuery.GetByIdAsync(userId, cancellationToken);

		if (user == null)
		{
			return Result.Failure<UserDetailDto>(UserErrors.NotFound);
		}

		if(user.Role.Code == RoleCodes.SystemAdministrator)
		{
			return Result.Failure<UserDetailDto>(UserErrors.UnauthorizedResourceAccess);
		}

		// Process avatar URL if profile exists
		if (user.Profile?.Avatar != null)
		{
			var avatarUrl = await _storageService.CreatePresignedReadUrlAsync(user.Profile.Avatar, cancellationToken);

			var updatedUser = user with { Profile = user.Profile with { Avatar = avatarUrl } };

			return Result.Success(updatedUser);
		}

		return Result.Success(user);
	}
}
