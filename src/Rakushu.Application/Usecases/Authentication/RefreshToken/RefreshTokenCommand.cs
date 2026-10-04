using MediatR;
using Rakushu.Application.Usecases.Authentication.Login;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<CredentialResponseDto>>;
