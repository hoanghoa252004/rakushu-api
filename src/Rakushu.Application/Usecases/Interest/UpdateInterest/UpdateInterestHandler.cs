using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;
using Rakushu.Domain.Entities.User.Profile.Interest;

namespace Rakushu.Application.Usecases.Interest.UpdateInterest;

internal sealed class UpdateInterestHandler : IRequestHandler<UpdateInterestCommand, Result>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateInterestHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateInterestCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var interestId = InterestId.From(request.InterestId);
			var users = await _userRepository.FindAsync(u => u.Profile != null && u.Profile.Interests.Any(i => i.Id == interestId), cancellationToken);
			var user = users.FirstOrDefault();
			if (user is null || user.Profile is null)
				return Result.Failure(ProfileErrors.InterestNotFound);

			return user.Profile.UpdateInterest(interestId, request.priority);
		}, cancellationToken);
	}
}