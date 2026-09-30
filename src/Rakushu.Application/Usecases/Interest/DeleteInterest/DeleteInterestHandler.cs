using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;
using Rakushu.Domain.Entities.User.Profile.Interest;

namespace Rakushu.Application.Usecases.Interest.DeleteInterest;

internal sealed class DeleteInterestHandler : IRequestHandler<DeleteInterestCommand, Result>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteInterestHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteInterestCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var interestId = InterestId.From(request.InterestId);
			var users = await _userRepository.FindAsync(u => u.Profile != null && u.Profile.Interests.Any(i => i.Id == interestId), cancellationToken);
			var user = users.FirstOrDefault();
			if (user is null || user.Profile is null)
				return Result.Failure(ProfileErrors.InterestNotFound);

			return user.Profile.RemoveInterest(interestId);
		}, cancellationToken);
	}
}