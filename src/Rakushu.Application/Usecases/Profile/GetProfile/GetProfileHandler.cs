using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
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

	public async Task<Result<UserDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;

		var user = await _userQuery.GetByIdAsync(userId, cancellationToken);

		if (user == null)
		{
			return Result.Failure<UserDto>(UserError.NotFound);
		}

		var avatarUrl = user.AvatarUrl == null
			? null
			: await _storageService.CreatePresignedReadUrlAsync(user.AvatarUrl, cancellationToken);

		var dto = new UserDto(
			user.Id,
			user.Email,
			user.FullName,
			user.Role,
			user.Status,
			user.CreatedAt,
			user.UpdatedAt,
			avatarUrl,
			user.NativeLanguage
			);

		return Result.Success(dto);
	}
}
