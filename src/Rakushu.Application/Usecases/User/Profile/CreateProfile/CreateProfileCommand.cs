using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.User.Profile.CreateProfile;

public sealed record CreateProfileCommand(
	string FullName,
	Guid NativeLanguageId,
	Guid CurrentLevelId,
	Guid TargetLevelId,
	int DailyLearningMinutes,
	int SessionDurationMinutes,
	IReadOnlyCollection<CreateInterestDto> Interests,
	string? AvatarKey = null
) : IRequest<Result<Guid>>;

public sealed record CreateInterestDto(
	Guid ContentCategoryId,
	int Priority
);
