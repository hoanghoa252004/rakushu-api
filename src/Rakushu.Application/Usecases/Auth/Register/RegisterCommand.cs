using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Auth.Register;

public sealed record RegisterCommand(
	string Username,
	string Email,
	string Password,
	string? DisplayName = null,
	string? NativeLanguage = null,
	string? LearningLanguage = null
) : IRequest<Result<RegisterResponseDto>>;
