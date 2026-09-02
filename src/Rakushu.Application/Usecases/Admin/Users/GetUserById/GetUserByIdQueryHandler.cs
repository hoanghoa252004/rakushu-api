using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Admin.Users.GetUserById;

internal sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailDto>>
{
	private readonly IUserRepository _userRepository;

	public GetUserByIdQueryHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<Result<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId, cancellationToken);
		if (user is null)
		{
			return Result.Failure<UserDetailDto>(UserError.NotFound);
		}

		var profile = user.Profile;
		return Result.Success(new UserDetailDto(
			UserId: user.Id,
			Username: user.Username,
			Email: user.Email,
			RoleId: user.RoleId,
			RoleName: user.Role?.RoleName ?? "Learner",
			DisplayName: profile?.DisplayName ?? user.Username,
			AvatarUrl: profile?.AvatarUrl,
			Bio: profile?.Bio,
			NativeLanguage: profile?.NativeLanguage,
			LearningLanguage: profile?.LearningLanguage,
			Status: user.Status.ToString(),
			CreatedAt: user.CreatedAt,
			UpdatedAt: user.UpdatedAt
		));
	}
}
