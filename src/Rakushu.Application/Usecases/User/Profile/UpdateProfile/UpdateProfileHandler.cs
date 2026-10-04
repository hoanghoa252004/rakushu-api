using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;
using Rakushu.Domain.Entities.SupportedLanguage;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile.Policies;

namespace Rakushu.Application.Usecases.User.Profile.UpdateProfile;

internal sealed class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, Result>
{
	// USER CONTEXT
	private readonly ICurrentUserContext _currentUserContext;

	// REPOSITORIES
	private readonly IUserRepository _userRepository;
	private readonly ISupportedLanguageRepository _supportedLanguageRepository;
	private readonly IProficiencyLevelRepository _proficiencyLevelRepository;
	private readonly IContentCategoryRepository _contentCategoryRepository;
	private readonly IUnitOfWork _unitOfWork;

	// POLICY
	private readonly ProfileUpdatePolicy _profileUpdatePolicy;

	public UpdateProfileHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository,
		ISupportedLanguageRepository supportedLanguageRepository,
		IProficiencyLevelRepository proficiencyLevelRepository,
		IContentCategoryRepository contentCategoryRepository,
		IUnitOfWork unitOfWork,
		ProfileUpdatePolicy profileUpdatePolicy
		)
	{
		_currentUserContext = currentUserContext;
		_userRepository = userRepository;
		_supportedLanguageRepository = supportedLanguageRepository;
		_proficiencyLevelRepository = proficiencyLevelRepository;
		_contentCategoryRepository = contentCategoryRepository;
		_unitOfWork = unitOfWork;
		_profileUpdatePolicy = profileUpdatePolicy;
	}

	public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync( async () =>
		{
			// 1.1 VALIDATE: user authentication
			var userId = _currentUserContext.UserId;

			if(userId == null)
			{
				return Result.Failure(UserErrors.UnauthorizedResourceAccess);
			}

			var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

			if (user == null)
			{
				return Result.Failure(UserErrors.NotFound);
			}

			if(user.Profile == null)
			{
				return Result.Failure(UserErrors.ProfileNotFound);
			}

			// 1.2. VALIDATE: resource existence
			// 1.2.1 Native Language
			var nativeLanguageId = SupportedLanguageId.From(request.NativeLanguageId);

			var nativeLanguage = await _supportedLanguageRepository.GetByIdAsync(nativeLanguageId, cancellationToken);

			if (nativeLanguage == null)
			{
				return Result.Failure(SupportedLanguageErrors.NotFound);
			}

			// 1.2.2 Current Level
			var currentLevelId = ProficiencyLevelId.From(request.CurrentLevelId);

			var currentLevel = await _proficiencyLevelRepository.GetByIdAsync(currentLevelId, cancellationToken);

			if (currentLevel == null)
			{
				return Result.Failure(ProficiencyLevelErrors.NotFound);
			}

			// 1.2.3 Target Level
			var targetLevelId = ProficiencyLevelId.From(request.TargetLevelId);

			var targetLevel = await _proficiencyLevelRepository.GetByIdAsync(targetLevelId, cancellationToken);

			if (targetLevel == null)
			{
				return Result.Failure(ProficiencyLevelErrors.NotFound);
			}

			// 1.2.4 Content Category if interests are provided
			var contentCategories = new List<ContentCategory>();

			
			var categoryIds = request.Interests.Select(i => i.ContentCategoryId).ToList();

			foreach (var categoryId in categoryIds)
			{
				var category = await _contentCategoryRepository.GetByIdAsync(
					ContentCategoryId.From(categoryId), 
					cancellationToken);

				if (category == null)
				{
					return Result.Failure(ContentCategoryErrors.NotFound);
				}

				contentCategories.Add(category);
			}
			

			// 2. BUSINESS RULES VALIDATION
			var policyResult = _profileUpdatePolicy.Validate(
				user,
				nativeLanguage,
				currentLevel,
				targetLevel,
				contentCategories
			);

			if(policyResult.IsFailure)
			{
				return policyResult;
			}

			// 3. Update the profile with all fields
			var updateResult = user.Profile.Update(
				request.FullName,
				nativeLanguageId,
				currentLevelId,
				targetLevelId,
				request.DailyLearningMinutes,
				request.SessionDurationMinutes,
				DateTimeOffset.UtcNow,
				request.AvatarKey
			);

			if (updateResult.IsFailure)
			{
				return updateResult;
			}

			// 4. Update interests if provided
			
			var interests = new List<Rakushu.Domain.Entities.User.Profile.Interest.Interest>();

			foreach (var x in request.Interests)
			{
				var interestResult = Rakushu.Domain.Entities.User.Profile.Interest.Interest
					.Create( user.Profile.Id, ContentCategoryId.From(x.ContentCategoryId), x.Priority);

				if (interestResult.IsFailure)
				{
					return interestResult;
				}

				interests.Add(interestResult.Value);
			}

			var result = user.Profile.UpdateInterests(interests);

			if (result.IsFailure)
			{
				return result;
			}
			

			return Result.Success();
		}, cancellationToken);
	}
}
