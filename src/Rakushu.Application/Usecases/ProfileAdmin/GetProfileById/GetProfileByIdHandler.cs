using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Application.Usecases.ProfileAdmin.GetProfileById;

internal sealed class GetProfileByIdHandler : IRequestHandler<GetProfileByIdQuery, Result<ProfileDto>>
{
	private readonly IUserRepository _userRepository;

	public GetProfileByIdHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<Result<ProfileDto>> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
	{
		var users = await _userRepository.FindAsync(u => u.Profile != null && u.Profile.Id == ProfileId.From(request.ProfileId), cancellationToken);
		var user = users.FirstOrDefault();
		if (user is null || user.Profile is null)
			return Result.Failure<ProfileDto>(ProfileErrors.NotFound);

		return Result.Success(ProfileDto.FromEntity(user.Profile));
	}
}