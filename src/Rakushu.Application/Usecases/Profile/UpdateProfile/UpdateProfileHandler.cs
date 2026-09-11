using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.ValueObjects.Email;
using UserProfile = Rakushu.Domain.Entities.User.ValueObjects.Profile.Profile;

namespace Rakushu.Application.Usecases.Profile.UpdateProfile;

internal sealed class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, Result>
{
	// USER CONTEXT
	private readonly ICurrentUserContext _currentUserContext;

	// DAOs
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateProfileHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository,
		IUnitOfWork unitOfWork
		)
	{
		_currentUserContext = currentUserContext;
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync( async () =>
		{
			// 1. Get the current user 
			var userId = _currentUserContext.UserId;

			var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

			if (user == null)
			{
				return Result.Failure(UserError.NotFound);
			}

			// 2. Replace the previous profile with new profile
			var profileResult = UserProfile.Create(
				request.FullName, 
				request.NativeLanguage, 
				request.AvatarKey
				);

			if (profileResult.IsFailure)
			{
				return profileResult;
			}

			user.UpdateProfile(profileResult.Value);

			return Result.Success();
		}, cancellationToken);
	}
}
