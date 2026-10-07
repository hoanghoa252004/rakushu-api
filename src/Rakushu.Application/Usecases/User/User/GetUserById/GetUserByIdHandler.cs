using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.User.User.GetUserById;

internal sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailDto>>
{
	// DAOs
	private readonly IUserRepository _userRepository;

	// SERVICES
	private readonly IStorageService _storageService;

	public GetUserByIdHandler(
		IUserRepository userRepository,
		IStorageService storageService
		)
	{
		_userRepository = userRepository;
		_storageService = storageService;
	}

	public async Task<Result<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var userId = UserId.From(request.UserId);

		var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

		if (user == null)
		{
			return Result.Failure<UserDetailDto>(UserErrors.NotFound);
		}

		if(user.Role.Code == RoleCodes.SystemAdministrator)
		{
			return Result.Failure<UserDetailDto>(UserErrors.UnauthorizedResourceAccess);
		}

		var userDetailDto = new UserDetailDto
		(
			Id: user.Id.Value,
			FullName: user.FullName,
			Email: user.Email,
			Status: user.Status.ToString(),
			CreatedAt: user.CreatedAt,
			UpdatedAt: user.UpdatedAt,
			Role: new RoleDto(
				user.Role.Id.Value,
				user.Role.Code,
				user.Role.Name
			),
			Profile: user.Profile != null ? new ProfileDto
			(
				Id: user.Profile.Id.Value,
				Avatar: user.Profile.AvatarKey,
				DailyLearningMinutes: user.Profile.DailyLearningMinutes,
				SessionDurationMinutes: user.Profile.SessionDurationMinutes,
				Level: new LevelDto(
					Id: user.Profile.Level.Id.Value,
					Code: user.Profile.Level.Code,
					Name: user.Profile.Level.Name,
					JapaneseName: user.Profile.Level.JapaneseName,
					Description: user.Profile.Level.Description
				),
				Interests: user.Profile.Interests.Select(i => new InterestDto
				(
					Id: i.Id.Value,
					Priority: i.Priority,
					Content: new InterestedContentDto(
						Id: i.ContentCategory.Id.Value,
						Slug: i.ContentCategory.Slug,
						Code: i.ContentCategory.Code,
						Name: i.ContentCategory.Name,
						JapaneseName: i.ContentCategory.JapaneseName,
						ThemeColor: i.ContentCategory.ThemeColor,
						Description: i.ContentCategory.Description
					)
				)).ToList()
			) : null
		);

		// Process avatar URL if profile exists
		if (userDetailDto.Profile?.Avatar != null)
		{
			var avatarUrl = await _storageService.CreatePresignedReadUrlAsync(userDetailDto.Profile.Avatar, cancellationToken);

			var updatedProfile = userDetailDto.Profile with { Avatar = avatarUrl };

			userDetailDto = userDetailDto with { Profile = updatedProfile };
		}

		return Result.Success(userDetailDto);
	}
}
