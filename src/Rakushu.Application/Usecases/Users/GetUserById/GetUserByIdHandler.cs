using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Users.GetUserById;

internal sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
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

	public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var userId = UserId.From(request.UserId);

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
