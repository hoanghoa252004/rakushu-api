using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Authentication.Logout;

public sealed record LogoutCommand() : IRequest<Result>;
