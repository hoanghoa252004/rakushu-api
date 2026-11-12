using FluentValidation;
using MediatR;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Application.Common.Parsing;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessions;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingSessionInvitations;

// ---- CREATE (HOST) ----
public sealed record CreateShadowingSessionInvitationsCommand(Guid SessionId, IReadOnlyList<Guid> LearnerIds)
	: IRequest<Result<IReadOnlyList<Guid>>>;

internal sealed class CreateShadowingSessionInvitationsValidator : AbstractValidator<CreateShadowingSessionInvitationsCommand>
{
	public CreateShadowingSessionInvitationsValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.LearnerIds).NotEmpty().Must(l => l.Count <= 50).WithMessage("At most 50 learners per request");
		RuleForEach(x => x.LearnerIds).NotEmpty();
	}
}

internal sealed class CreateShadowingSessionInvitationsHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingSessionMemberRepository memberRepository,
	IShadowingSessionInvitationRepository invitationRepository,
	IUserRepository userRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateShadowingSessionInvitationsCommand, Result<IReadOnlyList<Guid>>>
{
	public async Task<Result<IReadOnlyList<Guid>>> Handle(
		CreateShadowingSessionInvitationsCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
			if (session is null)
				return Result.Failure<IReadOnlyList<Guid>>(ShadowingSessionErrors.NotFound);
			if (currentUser.UserId is null || session.HostId != currentUser.UserId)
				return Result.Failure<IReadOnlyList<Guid>>(ShadowingSessionErrors.NotHost);
			if (session.Status != ShadowingSessionStatus.Scheduled)
				return Result.Failure<IReadOnlyList<Guid>>(ShadowingSessionErrors.NotScheduled);

			var now = DateTimeOffset.UtcNow;
			var expiredAt = session.ScheduledStartAt.Add(ShadowingSession.ExpirationGracePeriod);
			if (expiredAt <= now)
				return Result.Failure<IReadOnlyList<Guid>>(ShadowingSessionInvitationErrors.InvalidExpiry);

			var created = new List<ShadowingSessionInvitation>();
			foreach (var rawId in request.LearnerIds.Distinct())
			{
				var learnerId = UserId.From(rawId);
				if (learnerId == session.HostId)
					return Result.Failure<IReadOnlyList<Guid>>(ShadowingSessionInvitationErrors.CannotInviteHost);
				if (await userRepository.GetByIdAsync(learnerId, cancellationToken) is null)
					return Result.Failure<IReadOnlyList<Guid>>(ShadowingSessionInvitationErrors.LearnerNotFound);
				if (await memberRepository.ExistsAsync(session.Id, learnerId, cancellationToken))
					return Result.Failure<IReadOnlyList<Guid>>(ShadowingSessionInvitationErrors.AlreadyMember);
				if (await invitationRepository.PendingExistsAsync(session.Id, learnerId, cancellationToken))
					return Result.Failure<IReadOnlyList<Guid>>(ShadowingSessionInvitationErrors.AlreadyInvited);

				created.Add(ShadowingSessionInvitation.Create(session.Id, session.HostId, learnerId, now, expiredAt));
			}

			foreach (var invitation in created)
				invitationRepository.Add(invitation);

			return Result.Success<IReadOnlyList<Guid>>(created.Select(i => i.Id.Value).ToList());
		}, cancellationToken);
	}
}

// ---- RESPOND (LEARNER) ----
public sealed record RespondShadowingSessionInvitationCommand(Guid InvitationId, string Status)
	: IRequest<Result>;

internal sealed class RespondShadowingSessionInvitationValidator : AbstractValidator<RespondShadowingSessionInvitationCommand>
{
	public RespondShadowingSessionInvitationValidator()
	{
		RuleFor(x => x.InvitationId).NotEmpty();
		RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required");
	}
}

internal sealed class RespondShadowingSessionInvitationHandler(
	IShadowingSessionInvitationRepository invitationRepository,
	IShadowingSessionRepository sessionRepository,
	IShadowingSessionMemberRepository memberRepository,
	IShadowingRoleRepository roleRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<RespondShadowingSessionInvitationCommand, Result>
{
	public async Task<Result> Handle(RespondShadowingSessionInvitationCommand request, CancellationToken cancellationToken)
	{
		var result = await unitOfWork.ExecuteAsync(async () =>
		{
			var status = EnumParser.Parse<SessionInvitationStatus>(request.Status, "status");
			if (status.IsFailure)
				return Result.Failure(status.Error);

			var invitation = await invitationRepository.GetByIdAsync(
				ShadowingSessionInvitationId.From(request.InvitationId), cancellationToken);
			// Hide others' invitations.
			if (invitation is null || currentUser.UserId is null || invitation.LearnerId != currentUser.UserId)
				return Result.Failure(ShadowingSessionInvitationErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var canRespond = invitation.EnsureCanRespond(now);
			if (canRespond.IsFailure)
				return canRespond;

			if (status.Value == SessionInvitationStatus.Accepted)
			{
				var session = await sessionRepository.GetByIdAsync(invitation.ShadowingSessionId, cancellationToken);
				if (session is null)
					return Result.Failure(ShadowingSessionErrors.NotFound);
				if (session.Status != ShadowingSessionStatus.Scheduled)
					return Result.Failure(ShadowingSessionErrors.NotScheduled);
				if (await memberRepository.ExistsAsync(session.Id, invitation.LearnerId, cancellationToken))
					return Result.Failure(ShadowingSessionMemberErrors.AlreadyMember);

				var roles = await roleRepository.GetByMaterialIdAsync(session.ShadowingMaterialId, cancellationToken);
				if (await memberRepository.CountBySessionAsync(session.Id, cancellationToken) >= roles.Count)
					return Result.Failure(ShadowingSessionErrors.SessionFull);

				var duration = await sessionRepository.GetDurationAsync(session.Id, cancellationToken);
				if (duration is null)
					return Result.Failure(ShadowingSessionErrors.NotFound);
				if (await sessionRepository.HasScheduleConflictAsync(
					invitation.LearnerId, session.ScheduledStartAt, duration.Value, session.Id, cancellationToken))
					return Result.Failure(ShadowingSessionErrors.ScheduleConflict);

				invitation.Respond(status.Value, now);

				memberRepository.Add(ShadowingSessionMember.CreateFromInvitation(
					session.Id, invitation.LearnerId, invitation.Id, now));
			}
			else
			{
				var responded = invitation.Respond(status.Value, now);
				if (responded.IsFailure)
					return responded;
			}

			return Result.Success();
		}, cancellationToken);

		return result;
	}
}

// ---- UPDATE LEARNER (HOST, PENDING ONLY) ----
public sealed record UpdateShadowingSessionInvitationCommand(Guid InvitationId, Guid LearnerId) : IRequest<Result>;

internal sealed class UpdateShadowingSessionInvitationValidator : AbstractValidator<UpdateShadowingSessionInvitationCommand>
{
	public UpdateShadowingSessionInvitationValidator()
	{
		RuleFor(x => x.InvitationId).NotEmpty();
		RuleFor(x => x.LearnerId).NotEmpty();
	}
}

internal sealed class UpdateShadowingSessionInvitationHandler(
	IShadowingSessionInvitationRepository invitationRepository,
	IShadowingSessionMemberRepository memberRepository,
	IUserRepository userRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<UpdateShadowingSessionInvitationCommand, Result>
{
	public async Task<Result> Handle(UpdateShadowingSessionInvitationCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var invitation = await invitationRepository.GetByIdAsync(
				ShadowingSessionInvitationId.From(request.InvitationId), cancellationToken);
			if (invitation is null)
				return Result.Failure(ShadowingSessionInvitationErrors.NotFound);
			if (currentUser.UserId is null || invitation.HostId != currentUser.UserId)
				return Result.Failure(ShadowingSessionInvitationErrors.NotHost);
			if (invitation.Status != SessionInvitationStatus.Pending)
				return Result.Failure(ShadowingSessionInvitationErrors.NotPending);

			var learnerId = UserId.From(request.LearnerId);
			if (learnerId == invitation.LearnerId)
				return Result.Success();
			if (learnerId == invitation.HostId)
				return Result.Failure(ShadowingSessionInvitationErrors.CannotInviteHost);
			if (await userRepository.GetByIdAsync(learnerId, cancellationToken) is null)
				return Result.Failure(ShadowingSessionInvitationErrors.LearnerNotFound);
			if (await memberRepository.ExistsAsync(invitation.ShadowingSessionId, learnerId, cancellationToken))
				return Result.Failure(ShadowingSessionInvitationErrors.AlreadyMember);
			if (await invitationRepository.PendingExistsAsync(invitation.ShadowingSessionId, learnerId, cancellationToken))
				return Result.Failure(ShadowingSessionInvitationErrors.AlreadyInvited);

			return invitation.ChangeLearner(learnerId, DateTimeOffset.UtcNow);
		}, cancellationToken);
	}
}

// ---- DELETE (HOST, PENDING ONLY) ----
public sealed record DeleteShadowingSessionInvitationCommand(Guid InvitationId) : IRequest<Result>;

internal sealed class DeleteShadowingSessionInvitationValidator : AbstractValidator<DeleteShadowingSessionInvitationCommand>
{
	public DeleteShadowingSessionInvitationValidator() => RuleFor(x => x.InvitationId).NotEmpty();
}

internal sealed class DeleteShadowingSessionInvitationHandler(
	IShadowingSessionInvitationRepository invitationRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteShadowingSessionInvitationCommand, Result>
{
	public async Task<Result> Handle(DeleteShadowingSessionInvitationCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var invitation = await invitationRepository.GetByIdAsync(
				ShadowingSessionInvitationId.From(request.InvitationId), cancellationToken);
			if (invitation is null)
				return Result.Failure(ShadowingSessionInvitationErrors.NotFound);
			if (currentUser.UserId is null || invitation.HostId != currentUser.UserId)
				return Result.Failure(ShadowingSessionInvitationErrors.NotHost);
			if (invitation.Status != SessionInvitationStatus.Pending)
				return Result.Failure(ShadowingSessionInvitationErrors.NotPending);

			invitationRepository.Delete(invitation);
			return Result.Success();
		}, cancellationToken);
	}
}

// ---- GET MY INVITATIONS (LEARNER) ----
public sealed record GetMyShadowingSessionInvitationsQuery(
	string? Status,
	int PageNumber,
	int PageSize) : IRequest<Result<PaginatedList<ShadowingSessionInvitationDto>>>;

internal sealed class GetMyShadowingSessionInvitationsValidator : AbstractValidator<GetMyShadowingSessionInvitationsQuery>
{
	public GetMyShadowingSessionInvitationsValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetMyShadowingSessionInvitationsHandler(
	IShadowingSessionInvitationRepository invitationRepository,
	IShadowingSessionRepository sessionRepository,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetMyShadowingSessionInvitationsQuery, Result<PaginatedList<ShadowingSessionInvitationDto>>>
{
	public async Task<Result<PaginatedList<ShadowingSessionInvitationDto>>> Handle(
		GetMyShadowingSessionInvitationsQuery request, CancellationToken cancellationToken)
	{
		if (currentUser.UserId is null)
			return Result.Failure<PaginatedList<ShadowingSessionInvitationDto>>(ShadowingSessionErrors.Unauthenticated);

		var parsedStatus = EnumParser.ParseOptional<SessionInvitationStatus>(request.Status, "status");
		if (parsedStatus.IsFailure)
			return Result.Failure<PaginatedList<ShadowingSessionInvitationDto>>(parsedStatus.Error);

		var (items, total) = await invitationRepository.GetPagedByLearnerAsync(
			currentUser.UserId, parsedStatus.Value, request.PageNumber, request.PageSize, cancellationToken);

		var dtos = new List<ShadowingSessionInvitationDto>();
		foreach (var item in items)
		{
			var session = await sessionRepository.GetByIdAsync(item.ShadowingSessionId, cancellationToken);
			dtos.Add(ShadowingSessionInvitationDto.FromEntity(item, session));
		}

		return Result.Success(PaginatedList<ShadowingSessionInvitationDto>.Create(
			dtos, total, request.PageNumber, request.PageSize));
	}
}

// ---- EXPIRE SESSIONS + THEIR INVITATIONS (WORKER) ----
public sealed record ExpireShadowingSessionsCommand(int BatchSize) : IRequest<Result<int>>;

internal sealed class ExpireShadowingSessionsHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingSessionInvitationRepository invitationRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<ExpireShadowingSessionsCommand, Result<int>>
{
	public async Task<Result<int>> Handle(ExpireShadowingSessionsCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var now = DateTimeOffset.UtcNow;
			var sessions = await sessionRepository.GetExpiredScheduledAsync(now, request.BatchSize, cancellationToken);
			var count = 0;
			foreach (var session in sessions)
			{
				if (session.Expire().IsFailure)
					continue;
				count++;
				var pending = await invitationRepository.GetPendingBySessionAsync(session.Id, cancellationToken);
				foreach (var invitation in pending)
					invitation.MarkExpired(now);
			}
			return Result.Success(count);
		}, cancellationToken);
	}
}
