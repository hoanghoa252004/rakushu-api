using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Authentication.Register;

public sealed record RegisterCommand(
	string FullName,
	string Email,
	string Password
) : IRequest<Result>;
