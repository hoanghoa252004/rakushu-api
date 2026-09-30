using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;
using UserProfile = Rakushu.Domain.Entities.User.Profile.Profile;
using Rakushu.Domain.SupportedLanguage;

namespace Rakushu.Application.Usecases.ProfileAdmin.CreateProfile;

internal sealed class CreateProfileHandler : IRequestHandler<CreateProfileCommand, Result<Guid>>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateProfileHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var user = await _userRepository.GetByIdAsync(UserId.From(request.userId), cancellationToken);
			if (user is null)
				return Result.Failure<Guid>(UserError.NotFound);

			if (user.Profile is not null)
				return Result.Failure<Guid>(ProfileErrors.AlreadyExists);

			var now = DateTimeOffset.UtcNow;
			var profileResult = UserProfile.Create(
				user.Id,
				request.fullName,
				SupportedLanguageId.From(request.nativeLanguageId),
				ProficiencyLevelId.From(request.currentLevelId),
				ProficiencyLevelId.From(request.targetLevelId),
				request.dailyLearningMinutes,
				request.sessionDurationMinutes,
				now,
				now,
				request.avatarKey);

			if (profileResult.IsFailure)
				return Result.Failure<Guid>(profileResult.Error);

			user.SetProfile(profileResult.Value);
			return Result.Success(profileResult.Value.Id.Value);
		}, cancellationToken);
	}
}