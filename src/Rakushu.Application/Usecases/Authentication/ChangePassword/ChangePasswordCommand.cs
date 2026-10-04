using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Authentication.ChangePassword;

public sealed record ChangePasswordCommand(
	string CurrentPassword,
	string NewPassword
) : IRequest<Result>;
