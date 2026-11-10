using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingSessionMembers;

// ---- GET MEMBERS OF A SESSION ----
public sealed record ShadowingSessionMemberDto(
	Guid Id,
	Guid MemberId,
	Guid? RoleId,
	string JoinType,
	bool IsHost,
	DateTimeOffset JoinedAt);

public sealed record GetShadowingSessionMembersQuery(Guid SessionId) : IRequest<Result<IReadOnlyList<ShadowingSessionMemberDto>>>;

internal sealed class GetShadowingSessionMembersValidator : AbstractValidator<GetShadowingSessionMembersQuery>
{
	public GetShadowingSessionMembersValidator() => RuleFor(x => x.SessionId).NotEmpty();
}

internal sealed class GetShadowingSessionMembersHandler(
	IShadowingSessionRepository sessionRepository,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingSessionMembersQuery, Result<IReadOnlyList<ShadowingSessionMemberDto>>>
{
	public async Task<Result<IReadOnlyList<ShadowingSessionMemberDto>>> Handle(
		GetShadowingSessionMembersQuery request, CancellationToken cancellationToken)
	{
		if (currentUser.UserId is null)
			return Result.Failure<IReadOnlyList<ShadowingSessionMemberDto>>(ShadowingSessionErrors.Unauthenticated);

		var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
		if (session is null)
			return Result.Failure<IReadOnlyList<ShadowingSessionMemberDto>>(ShadowingSessionErrors.NotFound);

		var isCurator = currentUser.Role == Rakushu.Domain.Entities.Identity.Role.RoleCodes.LinguisticCurator;
		if (!isCurator && !session.Members.Any(m => m.MemberId == currentUser.UserId))
			return Result.Failure<IReadOnlyList<ShadowingSessionMemberDto>>(ShadowingSessionErrors.NotMember);

		IReadOnlyList<ShadowingSessionMemberDto> items = session.Members
			.OrderBy(m => m.JoinedAt)
			.Select(m => new ShadowingSessionMemberDto(
				m.Id.Value, m.MemberId.Value, m.ShadowingRoleId?.Value, m.JoinType.ToString(),
				m.MemberId == session.HostId, m.JoinedAt))
			.ToList();
		return Result.Success(items);
	}
}

// ---- JOIN PUBLIC SESSION ----
public sealed record JoinShadowingSessionCommand(Guid SessionId) : IRequest<Result<Guid>>;

internal sealed class JoinShadowingSessionValidator : AbstractValidator<JoinShadowingSessionCommand>
{
	public JoinShadowingSessionValidator() => RuleFor(x => x.SessionId).NotEmpty();
}

internal sealed class JoinShadowingSessionHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingSessionMemberRepository memberRepository,
	IShadowingRoleRepository roleRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<JoinShadowingSessionCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(JoinShadowingSessionCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var userId = currentUser.UserId;
			if (userId is null)
				return Result.Failure<Guid>(ShadowingSessionErrors.Unauthenticated);

			var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
			if (session is null)
				return Result.Failure<Guid>(ShadowingSessionErrors.NotFound);
			if (session.Visibility != ShadowingSessionVisibility.Public)
				return Result.Failure<Guid>(ShadowingSessionErrors.NotPublic);
			if (session.Status != ShadowingSessionStatus.Scheduled)
				return Result.Failure<Guid>(ShadowingSessionErrors.NotScheduled);
			if (await memberRepository.ExistsAsync(session.Id, userId, cancellationToken))
				return Result.Failure<Guid>(ShadowingSessionMemberErrors.AlreadyMember);

			var roles = await roleRepository.GetByMaterialIdAsync(session.ShadowingMaterialId, cancellationToken);
			if (await memberRepository.CountBySessionAsync(session.Id, cancellationToken) >= roles.Count)
				return Result.Failure<Guid>(ShadowingSessionErrors.SessionFull);

			var duration = await sessionRepository.GetDurationAsync(session.Id, cancellationToken);
			if (duration is null)
				return Result.Failure<Guid>(ShadowingSessionErrors.NotFound);
			if (await sessionRepository.HasScheduleConflictAsync(
				userId, session.ScheduledStartAt, duration.Value, session.Id, cancellationToken))
				return Result.Failure<Guid>(ShadowingSessionErrors.ScheduleConflict);

			var member = ShadowingSessionMember.CreatePublicJoin(session.Id, userId, DateTimeOffset.UtcNow);
			memberRepository.Add(member);
			return Result.Success(member.Id.Value);
		}, cancellationToken);
	}
}

// ---- REMOVE MEMBER (HOST) ----
public sealed record RemoveShadowingSessionMemberCommand(Guid SessionId, Guid MemberRecordId) : IRequest<Result>;

internal sealed class RemoveShadowingSessionMemberValidator : AbstractValidator<RemoveShadowingSessionMemberCommand>
{
	public RemoveShadowingSessionMemberValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.MemberRecordId).NotEmpty();
	}
}

internal sealed class RemoveShadowingSessionMemberHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingSessionMemberRepository memberRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<RemoveShadowingSessionMemberCommand, Result>
{
	public async Task<Result> Handle(RemoveShadowingSessionMemberCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
			if (session is null)
				return Result.Failure(ShadowingSessionErrors.NotFound);
			if (currentUser.UserId is null || session.HostId != currentUser.UserId)
				return Result.Failure(ShadowingSessionErrors.NotHost);
			if (session.Status != ShadowingSessionStatus.Scheduled)
				return Result.Failure(ShadowingSessionErrors.NotScheduled);

			var member = session.Members.FirstOrDefault(m => m.Id.Value == request.MemberRecordId);
			if (member is null)
				return Result.Failure(ShadowingSessionMemberErrors.NotFound);
			if (member.MemberId == session.HostId)
				return Result.Failure(ShadowingSessionMemberErrors.CannotRemoveHost);

			memberRepository.Delete(member);
			return Result.Success();
		}, cancellationToken);
	}
}

// ---- CHOOSE ROLE ----
public sealed record ChooseShadowingSessionRoleCommand(Guid SessionId, Guid MemberRecordId, Guid RoleId) : IRequest<Result>;

internal sealed class ChooseShadowingSessionRoleValidator : AbstractValidator<ChooseShadowingSessionRoleCommand>
{
	public ChooseShadowingSessionRoleValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.MemberRecordId).NotEmpty();
		RuleFor(x => x.RoleId).NotEmpty();
	}
}

internal sealed class ChooseShadowingSessionRoleHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingSessionMemberRepository memberRepository,
	IShadowingRoleRepository roleRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<ChooseShadowingSessionRoleCommand, Result>
{
	public async Task<Result> Handle(ChooseShadowingSessionRoleCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
			if (session is null)
				return Result.Failure(ShadowingSessionErrors.NotFound);
			if (session.Status != ShadowingSessionStatus.Scheduled)
				return Result.Failure(ShadowingSessionErrors.NotScheduled);

			var member = session.Members.FirstOrDefault(m => m.Id.Value == request.MemberRecordId);
			if (member is null)
				return Result.Failure(ShadowingSessionMemberErrors.NotFound);
			if (currentUser.UserId is null)
				return Result.Failure(ShadowingSessionErrors.Unauthenticated);
			var isHost = session.HostId == currentUser.UserId;
			if (!isHost && member.MemberId != currentUser.UserId)
				return Result.Failure(ShadowingSessionMemberErrors.NotOwner);

			var role = await roleRepository.GetByIdAsync(ShadowingRoleId.From(request.RoleId), cancellationToken);
			if (role is null)
				return Result.Failure(ShadowingSessionMemberErrors.RoleNotFound);
			if (role.ShadowingMaterialId != session.ShadowingMaterialId)
				return Result.Failure(ShadowingSessionMemberErrors.RoleNotInMaterial);
			if (await memberRepository.RoleTakenAsync(session.Id, role.Id, member.Id, cancellationToken))
				return Result.Failure(ShadowingSessionMemberErrors.RoleAlreadyTaken);

			member.ChooseRole(role.Id);
			return Result.Success();
		}, cancellationToken);
	}
}
