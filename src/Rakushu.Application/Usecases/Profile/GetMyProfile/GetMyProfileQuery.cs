using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Profile.GetMyProfile;

public sealed record GetMyProfileQuery : IRequest<Result<ProfileResponseDto>>;
