using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;
using Rakushu.Domain.Entities.User.Profile.Interest;

namespace Rakushu.Application.Usecases.Interest.GetInterestById;

internal sealed class GetInterestByIdHandler : IRequestHandler<GetInterestByIdQuery, Result<InterestDto>>
{
	private readonly IUserRepository _userRepository;

	public GetInterestByIdHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<Result<InterestDto>> Handle(GetInterestByIdQuery request, CancellationToken cancellationToken)
	{
		var interestId = InterestId.From(request.InterestId);
		var users = await _userRepository.FindAsync(u => u.Profile != null && u.Profile.Interests.Any(i => i.Id == interestId), cancellationToken);
		var user = users.FirstOrDefault();
		if (user is null || user.Profile is null)
			return Result.Failure<InterestDto>(ProfileErrors.InterestNotFound);

		var interest = user.Profile.Interests.FirstOrDefault(i => i.Id == interestId);
		if (interest is null)
			return Result.Failure<InterestDto>(ProfileErrors.InterestNotFound);

		return Result.Success(InterestDto.FromEntity(interest));
	}
}