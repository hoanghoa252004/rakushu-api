using MediatR;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Auth.Login;

internal sealed record LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
	public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		if (request.Email.Equals("hoathse184053@fpt.edu.vn", StringComparison.OrdinalIgnoreCase)
			&& request.Password.Equals("123456"))
			return Result.Success(new LoginResponseDto(
				AccessToken: "access_token",
				RefreshToken: "refresh_token",
				AccessTokenExpiresAt: DateTimeOffset.UtcNow.AddHours(1)
				));
		else
			return Result.Failure<LoginResponseDto>(Error.Unauthorized("AUTH.LOGIN.INVALID_CREDENTIAL", "Invalid email or password."));
	}
}