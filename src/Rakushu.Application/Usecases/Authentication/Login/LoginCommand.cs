using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Authentication.Login;

public sealed record LoginCommand(
	string Email,
	string Password
) : IRequest<Result<CredentialResponseDto>>;
