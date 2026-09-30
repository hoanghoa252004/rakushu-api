using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Interest.GetInterests;

internal sealed class GetInterestsHandler : IRequestHandler<GetInterestsQuery, Result<IReadOnlyCollection<InterestDto>>>
{
	private readonly IUserRepository _userRepository;

	public GetInterestsHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<Result<IReadOnlyCollection<InterestDto>>> Handle(GetInterestsQuery request, CancellationToken cancellationToken)
	{
		var users = await _userRepository.GetAllAsync(cancellationToken);
		var interests = users
			.Where(u => u.Profile != null)
			.SelectMany(u => u.Profile!.Interests)
			.Select(InterestDto.FromEntity)
			.ToArray();

		return Result.Success<IReadOnlyCollection<InterestDto>>(interests);
	}
}