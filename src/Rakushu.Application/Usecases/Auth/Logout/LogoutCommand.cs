using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Auth.Logout;

public sealed record LogoutCommand() : IRequest<Result>;
