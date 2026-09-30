using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Application.Usecases.ProfileAdmin.GetProfiles;

internal sealed class GetProfilesHandler : IRequestHandler<GetProfilesQuery, Result<IReadOnlyCollection<ProfileDto>>>
{
	private readonly IUserRepository _userRepository;

	public GetProfilesHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<Result<IReadOnlyCollection<ProfileDto>>> Handle(GetProfilesQuery request, CancellationToken cancellationToken)
	{
		var users = await _userRepository.GetAllAsync(cancellationToken);
		var profiles = users
			.Where(u => u.Profile != null)
			.Select(u => ProfileDto.FromEntity(u.Profile!))
			.ToArray();

		return Result.Success<IReadOnlyCollection<ProfileDto>>(profiles);
	}
}