using FluentValidation;
using MediatR;
using Rakushu.Application.Common.Parsing;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingSessions.UpdateShadowingSession;

public sealed record UpdateShadowingSessionCommand(
	Guid SessionId,
	string Title,
	string Visibility,
	DateTimeOffset ScheduledStartAt) : IRequest<Result>;

internal sealed class UpdateShadowingSessionValidator : AbstractValidator<UpdateShadowingSessionCommand>
{
	public UpdateShadowingSessionValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
		RuleFor(x => x.Visibility).NotEmpty();
	}
}

internal sealed class UpdateShadowingSessionHandler(
	IShadowingSessionRepository repository,
	IShadowingSessionInvitationRepository invitationRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<UpdateShadowingSessionCommand, Result>
{
	public async Task<Result> Handle(UpdateShadowingSessionCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var visibility = EnumParser.Parse<ShadowingSessionVisibility>(request.Visibility, "visibility");
			if (visibility.IsFailure)
				return Result.Failure(visibility.Error);

			var session = await repository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
			if (session is null)
				return Result.Failure(ShadowingSessionErrors.NotFound);
			if (currentUser.UserId is null || session.HostId != currentUser.UserId)
				return Result.Failure(ShadowingSessionErrors.NotHost);

			var now = DateTimeOffset.UtcNow;
			var updated = session.Update(request.Title, visibility.Value, request.ScheduledStartAt, now);
			if (updated.IsFailure)
				return updated;

			// Keep pending invitations aligned with the new start time.
			var pending = await invitationRepository.GetPendingBySessionAsync(session.Id, cancellationToken);
			foreach (var invitation in pending)
				invitation.UpdateExpiry(session.ScheduledStartAt.AddMinutes(-10), now);

			return Result.Success();
		}, cancellationToken);
	}
}
