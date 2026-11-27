using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Authentication.ResetPassword;

public sealed record ResetPasswordCommand(
	string Email,
	string Code,
	string NewPassword
) : IRequest<Result>;
