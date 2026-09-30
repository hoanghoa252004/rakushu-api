using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;
using Rakushu.Domain.SupportedLanguage;

namespace Rakushu.Application.Usecases.ProfileAdmin.UpdateProfile;

internal sealed class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, Result>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateProfileHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var user = await _userRepository.GetByIdAsync(UserId.From(request.userId), cancellationToken);
			if (user is null || user.Profile is null)
				return Result.Failure(ProfileErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var updateResult = user.Profile.Update(
				request.fullName,
				SupportedLanguageId.From(request.nativeLanguageId),
				ProficiencyLevelId.From(request.currentLevelId),
				ProficiencyLevelId.From(request.targetLevelId),
				request.dailyLearningMinutes,
				request.sessionDurationMinutes,
				now,
				request.avatarKey);

			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}