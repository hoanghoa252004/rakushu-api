using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingSessions.DeleteShadowingSession;

public sealed record DeleteShadowingSessionCommand(Guid SessionId) : IRequest<Result>;

internal sealed class DeleteShadowingSessionValidator : AbstractValidator<DeleteShadowingSessionCommand>
{
	public DeleteShadowingSessionValidator() => RuleFor(x => x.SessionId).NotEmpty();
}

internal sealed class DeleteShadowingSessionHandler(
	IShadowingSessionRepository repository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteShadowingSessionCommand, Result>
{
	public async Task<Result> Handle(DeleteShadowingSessionCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var session = await repository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
			if (session is null)
				return Result.Failure(ShadowingSessionErrors.NotFound);
			if (currentUser.UserId is null || session.HostId != currentUser.UserId)
				return Result.Failure(ShadowingSessionErrors.NotHost);
			if (session.Status != ShadowingSessionStatus.Scheduled)
				return Result.Failure(ShadowingSessionErrors.NotScheduled);
			if (session.Members.Any(m => m.MemberId != session.HostId))
				return Result.Failure(ShadowingSessionErrors.HasMembers);

			repository.Delete(session);
			return Result.Success();
		}, cancellationToken);
	}
}
