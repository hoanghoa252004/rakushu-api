using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Application.Usecases.ProfileAdmin.DeleteProfile;

internal sealed class DeleteProfileHandler : IRequestHandler<DeleteProfileCommand, Result>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteProfileHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var users = await _userRepository.FindAsync(u => u.Profile != null && u.Profile.Id == ProfileId.From(request.ProfileId), cancellationToken);
			var user = users.FirstOrDefault();
			if (user is null || user.Profile is null)
				return Result.Failure(ProfileErrors.NotFound);

			user.RemoveProfile();
			return Result.Success();
		}, cancellationToken);
	}
}