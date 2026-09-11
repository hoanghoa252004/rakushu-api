using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Auth.Register;

public sealed record RegisterCommand(
	string Email,
	string Password,
	string FullName,
	string NativeLanguage,
	string? AvatarKey = null
) : IRequest<Result>;
