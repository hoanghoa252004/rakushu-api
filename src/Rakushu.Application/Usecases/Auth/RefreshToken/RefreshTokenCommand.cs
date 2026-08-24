using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshTokenResponseDto>>;
