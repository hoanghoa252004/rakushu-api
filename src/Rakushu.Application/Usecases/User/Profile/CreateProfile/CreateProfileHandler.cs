using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.ProficiencyLevel;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.User.Profile.CreateProfile;

internal sealed class CreateProfileHandler : IRequestHandler<CreateProfileCommand, Result<Guid>>
{
	// USER CONTEXT
	private readonly ICurrentUserContext _currentUserContext;

	// REPOSITORIES
	private readonly IUserRepository _userRepository;
	private readonly IProficiencyLevelRepository _proficiencyLevelRepository;
	private readonly IContentCategoryRepository _contentCategoryRepository;
	private readonly IUnitOfWork _unitOfWork;

	// POLICY
	private readonly ProfileUpdatePolicy _profileUpdatePolicy;

	public CreateProfileHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository,
		IProficiencyLevelRepository proficiencyLevelRepository,
		IContentCategoryRepository contentCategoryRepository,
		IUnitOfWork unitOfWork,
		ProfileUpdatePolicy profileUpdatePolicy
		)
	{
		_currentUserContext = currentUserContext;
		_userRepository = userRepository;
		_proficiencyLevelRepository = proficiencyLevelRepository;
		_contentCategoryRepository = contentCategoryRepository;
		_unitOfWork = unitOfWork;
		_profileUpdatePolicy = profileUpdatePolicy;
	}

	public async Task<Result<Guid>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1.1 VALIDATE: user authentication
			var userId = _currentUserContext.UserId;

			if (userId == null)
			{
				return Result.Failure<Guid>(UserErrors.UnauthorizedResourceAccess);
			}

			var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

			if (user == null)
			{
				return Result.Failure<Guid>(UserErrors.NotFound);
			}

			if(user.Profile != null)
			{
				return Result.Failure<Guid>(UserErrors.ProfileAlreadyExists);
			}

			// 1.2. VALIDATE: resource existence

			// 1.2.1 Level
			var levelId = ProficiencyLevelId.From(request.CurrentLevelId);

			var level = await _proficiencyLevelRepository.GetByIdAsync(levelId, cancellationToken);

			if (level == null)
			{
				return Result.Failure<Guid>(ProficiencyLevelErrors.NotFound);
			}

			// 1.2.2 Content Category if interests are provided
			var contentCategories = new List<ContentCategory>();


			var categoryIds = request.Interests.Select(i => i.ContentCategoryId).ToList();

			foreach (var categoryId in categoryIds)
			{
				var category = await _contentCategoryRepository.GetByIdAsync(
					ContentCategoryId.From(categoryId),
					cancellationToken);

				if (category == null)
				{
					return Result.Failure<Guid>(ContentCategoryErrors.NotFound);
				}

				contentCategories.Add(category);
			}


			// 2. BUSINESS RULES VALIDATION
			var policyResult = _profileUpdatePolicy.Validate(
				user,
				level,
				contentCategories
			);

			if (policyResult.IsFailure)
			{
				return Result.Failure<Guid>(policyResult.Error);
			}

			// 3. Update the profile with all fields
			var createProfileResult = Rakushu.Domain.Entities.User.Profile.Profile.Create(
				user.Id,
				levelId,
				request.DailyLearningMinutes,
				request.SessionDurationMinutes,
				DateTimeOffset.UtcNow,
				request.AvatarKey
			);

			if (createProfileResult.IsFailure)
			{
				return Result.Failure<Guid>(createProfileResult.Error);
			}

			user.SetProfile(createProfileResult.Value);

			// 4. Update interests if provided

			var interests = new List<Rakushu.Domain.Entities.User.Profile.Interest.Interest>();

			foreach (var x in request.Interests)
			{
				var interestResult = Rakushu.Domain.Entities.User.Profile.Interest.Interest
					.Create(createProfileResult.Value.Id, ContentCategoryId.From(x.ContentCategoryId), x.Priority);

				if (interestResult.IsFailure)
				{
					return Result.Failure<Guid>(interestResult.Error);
				}

				interests.Add(interestResult.Value);
			}

			var result = createProfileResult.Value.AddInterests(interests);

			if (result.IsFailure)
			{
				return Result.Failure<Guid>(result.Error);
			}

			return Result.Success(createProfileResult.Value.Id.Value);
		}, cancellationToken);
	}
}
