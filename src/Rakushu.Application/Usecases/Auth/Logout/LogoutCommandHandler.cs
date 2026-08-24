using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Auth.Logout;

internal sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IUnitOfWork _unitOfWork;

	public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
	{
		_refreshTokenRepository = refreshTokenRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.RefreshToken))
		{
			return Result.Success();
		}

		var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
		if (token is not null && !token.IsRevoked)
		{
			token.Revoke();
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		return Result.Success();
	}
}
