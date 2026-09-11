using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Auth.Login;

public sealed record LoginCommand(
	string Email,
	string Password
) : IRequest<Result<CredentialResponseDto>>;
