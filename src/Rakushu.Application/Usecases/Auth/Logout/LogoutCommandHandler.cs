using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Auth.Logout;

internal sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public LogoutCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.RefreshToken))
		{
			return Result.Success();
		}

		var token = await _userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
		if (token is not null && !token.IsRevoked)
		{
			token.Revoke();
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		return Result.Success();
	}
}
