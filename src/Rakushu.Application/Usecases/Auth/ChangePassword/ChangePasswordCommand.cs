using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Auth.ChangePassword;

public sealed record ChangePasswordCommand(
	string CurrentPassword,
	string NewPassword
) : IRequest<Result>;
