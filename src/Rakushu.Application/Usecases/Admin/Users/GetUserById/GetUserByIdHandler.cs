using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Admin.Users.GetUserById;

internal sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
	// DAOs
	private readonly IUserQuery _userQuery;

	public GetUserByIdHandler(IUserQuery userQuery)
	{
		_userQuery = userQuery;
	}

	public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var userId = UserId.From(request.UserId);

		var user = await _userQuery.GetByIdAsync(userId, cancellationToken);

		if (user == null)
		{
			return Result.Failure<UserDto>(UserError.NotFound);
		}

		return Result.Success(user);
	}
}
