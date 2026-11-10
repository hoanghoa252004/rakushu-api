using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;
using AttemptDetailEntity = Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail.ShadowingAttemptDetail;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingAttempts;

// ---- CANCEL SESSION (HOST) ----
public sealed record CancelShadowingSessionCommand(Guid SessionId) : IRequest<Result>;

internal sealed class CancelShadowingSessionValidator : AbstractValidator<CancelShadowingSessionCommand>
{
	public CancelShadowingSessionValidator() => RuleFor(x => x.SessionId).NotEmpty();
}

internal sealed class CancelShadowingSessionHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingSessionInvitationRepository invitationRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CancelShadowingSessionCommand, Result>
{
	public async Task<Result> Handle(CancelShadowingSessionCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
			if (session is null)
				return Result.Failure(ShadowingSessionErrors.NotFound);
			if (currentUser.UserId is null || session.HostId != currentUser.UserId)
				return Result.Failure(ShadowingSessionErrors.NotHost);

			var cancelled = session.Cancel();
			if (cancelled.IsFailure)
				return cancelled;

			var now = DateTimeOffset.UtcNow;
			var pending = await invitationRepository.GetPendingBySessionAsync(session.Id, cancellationToken);
			foreach (var invitation in pending)
				invitation.Revoke(now);

			return Result.Success();
		}, cancellationToken);
	}
}

// ---- START SESSION (HOST) ----
public sealed record StartShadowingSessionCommand(Guid SessionId) : IRequest<Result>;

internal sealed class StartShadowingSessionValidator : AbstractValidator<StartShadowingSessionCommand>
{
	public StartShadowingSessionValidator() => RuleFor(x => x.SessionId).NotEmpty();
}

internal sealed class StartShadowingSessionHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingRoleRepository roleRepository,
	IShadowingAttemptRepository attemptRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<StartShadowingSessionCommand, Result>
{
	public async Task<Result> Handle(StartShadowingSessionCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var session = await sessionRepository.GetWithAttemptsAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
			if (session is null)
				return Result.Failure(ShadowingSessionErrors.NotFound);
			if (currentUser.UserId is null || session.HostId != currentUser.UserId)
				return Result.Failure(ShadowingSessionErrors.NotHost);
			if (session.Status != ShadowingSessionStatus.Scheduled)
				return Result.Failure(ShadowingSessionErrors.NotScheduled);

			var roles = await roleRepository.GetByMaterialIdAsync(session.ShadowingMaterialId, cancellationToken);
			if (session.Members.Count != roles.Count)
				return Result.Failure(ShadowingSessionErrors.MembersIncomplete);
			if (session.Members.Any(m => m.ShadowingRoleId is null))
				return Result.Failure(ShadowingSessionErrors.RolesNotAssigned);

			var now = DateTimeOffset.UtcNow;
			var started = session.Start(now);
			if (started.IsFailure)
				return started;

			foreach (var member in session.Members)
				attemptRepository.Add(ShadowingAttempt.Start(session.Id, member.Id, now));

			return Result.Success();
		}, cancellationToken);
	}
}

// ---- CHANGE ATTEMPT STATUS (LEARNER): CANCEL ----
public sealed record CancelShadowingAttemptCommand(Guid AttemptId) : IRequest<Result>;

internal sealed class CancelShadowingAttemptValidator : AbstractValidator<CancelShadowingAttemptCommand>
{
	public CancelShadowingAttemptValidator() => RuleFor(x => x.AttemptId).NotEmpty();
}

internal sealed class CancelShadowingAttemptHandler(
	IShadowingAttemptRepository attemptRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CancelShadowingAttemptCommand, Result>
{
	public async Task<Result> Handle(CancelShadowingAttemptCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var attempt = await attemptRepository.GetWithContextAsync(ShadowingAttemptId.From(request.AttemptId), cancellationToken);
			if (attempt is null)
				return Result.Failure(ShadowingAttemptErrors.NotFound);
			if (currentUser.UserId is null || attempt.ShadowingSessionMember.MemberId != currentUser.UserId)
				return Result.Failure(ShadowingAttemptErrors.NotOwner);
			if (attempt.ShadowingSession.Status != ShadowingSessionStatus.InProgress)
				return Result.Failure(ShadowingAttemptErrors.SessionNotInProgress);

			return attempt.Cancel(DateTimeOffset.UtcNow);
		}, cancellationToken);
	}
}

// ---- GET MY ROLE LINES OF A SESSION ----
public sealed record MyRoleLineDto(
	Guid Id,
	int SortOrder,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string? AudioUrl,
	bool IsDone,
	Guid? AttemptDetailId);

public sealed record GetMySessionRoleLinesQuery(Guid SessionId, int PageNumber, int PageSize)
	: IRequest<Result<PaginatedList<MyRoleLineDto>>>;

internal sealed class GetMySessionRoleLinesValidator : AbstractValidator<GetMySessionRoleLinesQuery>
{
	public GetMySessionRoleLinesValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetMySessionRoleLinesHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IShadowingAttemptRepository attemptRepository,
	IStorageService storage,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetMySessionRoleLinesQuery, Result<PaginatedList<MyRoleLineDto>>>
{
	public async Task<Result<PaginatedList<MyRoleLineDto>>> Handle(
		GetMySessionRoleLinesQuery request, CancellationToken cancellationToken)
	{
		if (currentUser.UserId is null)
			return Result.Failure<PaginatedList<MyRoleLineDto>>(ShadowingSessionErrors.Unauthenticated);

		var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
		if (session is null)
			return Result.Failure<PaginatedList<MyRoleLineDto>>(ShadowingSessionErrors.NotFound);

		var member = session.Members.FirstOrDefault(m => m.MemberId == currentUser.UserId);
		if (member is null)
			return Result.Failure<PaginatedList<MyRoleLineDto>>(ShadowingSessionErrors.NotMember);
		if (member.ShadowingRoleId is null)
			return Result.Failure<PaginatedList<MyRoleLineDto>>(ShadowingSessionErrors.RolesNotAssigned);

		var (lines, total) = await roleLineRepository.GetPagedAsync(
			session.ShadowingMaterialId, null, member.ShadowingRoleId, request.PageNumber, request.PageSize, cancellationToken);

		var details = new Dictionary<Guid, Guid>();
		var attempt = await attemptRepository.GetByMemberAsync(session.Id, currentUser.UserId, cancellationToken);
		if (attempt is not null)
		{
			foreach (var d in await attemptRepository.GetDetailsAsync(attempt.Id, cancellationToken))
				details[d.ShadowingRoleLineId.Value] = d.Id.Value;
		}

		var items = new List<MyRoleLineDto>();
		foreach (var line in lines)
		{
			var url = string.IsNullOrWhiteSpace(line.AudioKey)
				? null
				: await storage.CreatePresignedReadUrlAsync(line.AudioKey, cancellationToken);
			var hasDetail = details.TryGetValue(line.Id.Value, out var detailId);
			items.Add(new MyRoleLineDto(
				line.Id.Value, line.SortOrder, line.Text, line.StartTime, line.EndTime, url, hasDetail,
				hasDetail ? detailId : null));
		}

		return Result.Success(PaginatedList<MyRoleLineDto>.Create(items, total, request.PageNumber, request.PageSize));
	}
}

// ---- ATTEMPT DETAIL ----
public sealed record AttemptDetailDto(
	Guid Id,
	Guid AttemptId,
	Guid RoleLineId,
	string? AudioKey,
	string? AudioUrl,
	long? DurationMs,
	bool IsSkipped,
	DateTimeOffset? SubmittedAt);

internal static class AttemptDetailGuard
{
	// Loads and validates ownership / status for write operations.
	public static async Task<Result<ShadowingAttempt>> LoadWritableAsync(
		IShadowingAttemptRepository attemptRepository,
		ICurrentUserContext currentUser,
		Guid attemptId,
		CancellationToken cancellationToken)
	{
		var attempt = await attemptRepository.GetWithContextAsync(ShadowingAttemptId.From(attemptId), cancellationToken);
		if (attempt is null)
			return Result.Failure<ShadowingAttempt>(ShadowingAttemptErrors.NotFound);
		if (currentUser.UserId is null || attempt.ShadowingSessionMember.MemberId != currentUser.UserId)
			return Result.Failure<ShadowingAttempt>(ShadowingAttemptErrors.NotOwner);
		if (attempt.ShadowingSession.Status != ShadowingSessionStatus.InProgress)
			return Result.Failure<ShadowingAttempt>(ShadowingAttemptErrors.SessionNotInProgress);
		// A cancelled (or submitted) attempt can never accept details.
		if (attempt.Status != ShadowingAttemptStatus.InProgress)
			return Result.Failure<ShadowingAttempt>(ShadowingAttemptErrors.NotInProgress);
		return Result.Success(attempt);
	}
}

// POST
public sealed record CreateAttemptDetailCommand(
	Guid SessionId, Guid AttemptId, Guid RoleLineId, string? AudioKey, long? DurationMs, bool? IsSkipped)
	: IRequest<Result<Guid>>;

internal sealed class CreateAttemptDetailValidator : AbstractValidator<CreateAttemptDetailCommand>
{
	public CreateAttemptDetailValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.AttemptId).NotEmpty();
		RuleFor(x => x.RoleLineId).NotEmpty();
		RuleFor(x => x.AudioKey).MaximumLength(500);
	}
}

internal sealed class CreateAttemptDetailHandler(
	IShadowingAttemptRepository attemptRepository,
	IShadowingRoleLineRepository roleLineRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateAttemptDetailCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateAttemptDetailCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var loaded = await AttemptDetailGuard.LoadWritableAsync(
				attemptRepository, currentUser, request.AttemptId, cancellationToken);
			if (loaded.IsFailure)
				return Result.Failure<Guid>(loaded.Error);
			var attempt = loaded.Value;
			if (attempt.ShadowingSessionId.Value != request.SessionId)
				return Result.Failure<Guid>(ShadowingAttemptErrors.NotFound);

			var line = await roleLineRepository.GetByIdAsync(ShadowingRoleLineId.From(request.RoleLineId), cancellationToken);
			if (line is null)
				return Result.Failure<Guid>(ShadowingAttemptErrors.RoleLineNotFound);
			if (line.ShadowingRoleId != attempt.ShadowingSessionMember.ShadowingRoleId)
				return Result.Failure<Guid>(ShadowingAttemptErrors.RoleLineNotYours);

			if (await attemptRepository.GetDetailAsync(attempt.Id, line.Id, cancellationToken) is not null)
				return Result.Failure<Guid>(ShadowingAttemptErrors.DetailExists);

			// Collaborative shadowing: the immediately preceding line must already be done by its owner.
			var previous = await roleLineRepository.GetPreviousAsync(attempt.ShadowingSession.ShadowingMaterialId, line.SortOrder, cancellationToken);
			if (previous is not null
				&& !await attemptRepository.RoleLineDoneInSessionAsync(attempt.ShadowingSessionId, previous.Id, cancellationToken)
				&& !await attemptRepository.IsRoleAttemptCancelledAsync(attempt.ShadowingSessionId, previous.ShadowingRoleId, cancellationToken))
				return Result.Failure<Guid>(ShadowingAttemptErrors.PreviousLineNotCompleted);

			var created = AttemptDetailEntity.Create(
				attempt.Id, attempt.ShadowingSessionId, attempt.ShadowingSessionMember.MemberId.Value, line.Id,
				request.AudioKey, request.DurationMs, request.IsSkipped ?? false, DateTimeOffset.UtcNow);
			if (created.IsFailure)
				return Result.Failure<Guid>(created.Error);

			attemptRepository.AddDetail(created.Value);

			var existing = await attemptRepository.CountDetailsAsync(attempt.Id, cancellationToken);
			var (_, totalLines) = await roleLineRepository.GetPagedAsync(
				attempt.ShadowingSession.ShadowingMaterialId, null, attempt.ShadowingSessionMember.ShadowingRoleId, 1, 1, cancellationToken);
			if (existing + 1 >= totalLines)
			{
				var completed = attempt.Complete(DateTimeOffset.UtcNow);
				if (completed.IsFailure)
					return Result.Failure<Guid>(completed.Error);
			}

			return Result.Success(created.Value.Id.Value);
		}, cancellationToken);
	}
}

// GET
public sealed record GetAttemptDetailQuery(Guid SessionId, Guid AttemptId, Guid RoleLineId) : IRequest<Result<AttemptDetailDto>>;

internal sealed class GetAttemptDetailValidator : AbstractValidator<GetAttemptDetailQuery>
{
	public GetAttemptDetailValidator()
	{
		RuleFor(x => x.AttemptId).NotEmpty();
		RuleFor(x => x.RoleLineId).NotEmpty();
	}
}

internal sealed class GetAttemptDetailHandler(
	IShadowingAttemptRepository attemptRepository,
	IStorageService storage,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetAttemptDetailQuery, Result<AttemptDetailDto>>
{
	public async Task<Result<AttemptDetailDto>> Handle(GetAttemptDetailQuery request, CancellationToken cancellationToken)
	{
		var attempt = await attemptRepository.GetWithContextAsync(ShadowingAttemptId.From(request.AttemptId), cancellationToken);
		if (attempt is null)
			return Result.Failure<AttemptDetailDto>(ShadowingAttemptErrors.NotFound);
		if (attempt.ShadowingSessionId.Value != request.SessionId)
			return Result.Failure<AttemptDetailDto>(ShadowingAttemptErrors.NotFound);
		if (currentUser.UserId is null || attempt.ShadowingSessionMember.MemberId != currentUser.UserId)
			return Result.Failure<AttemptDetailDto>(ShadowingAttemptErrors.NotOwner);

		var detail = await attemptRepository.GetDetailAsync(
			attempt.Id, ShadowingRoleLineId.From(request.RoleLineId), cancellationToken);
		if (detail is null)
			return Result.Failure<AttemptDetailDto>(ShadowingAttemptErrors.DetailNotFound);

		var url = string.IsNullOrWhiteSpace(detail.AudioKey)
			? null
			: await storage.CreatePresignedReadUrlAsync(detail.AudioKey, cancellationToken);

		return Result.Success(new AttemptDetailDto(
			detail.Id.Value, attempt.Id.Value, detail.ShadowingRoleLineId.Value,
			detail.AudioKey, url, detail.DurationMs, detail.IsSkipped, detail.SubmittedAt));
	}
}

// ---- PROGRESS SUMMARY / MATERIAL SUMMARY ----
public sealed record MaterialRoleSummaryDto(Guid RoleId, Guid SpeakerId, int SortOrder, int TotalLines);

public sealed record MaterialSummaryDto(Guid MaterialId, int RoleCount, IReadOnlyList<MaterialRoleSummaryDto> Roles);

public sealed record SessionProgressSummaryDto(int TotalLines, int DoneLines, double Progress);

public sealed record SessionOverviewDto(SessionProgressSummaryDto Progress, MaterialSummaryDto Material);

internal static class SessionOverviewBuilder
{
	public static async Task<SessionOverviewDto> BuildAsync(
		ShadowingSession session,
		IShadowingRoleRepository roleRepository,
		IShadowingRoleLineRepository roleLineRepository,
		IShadowingAttemptRepository attemptRepository,
		CancellationToken cancellationToken)
	{
		var (lines, _) = await roleLineRepository.GetPagedAsync(
			session.ShadowingMaterialId, null, null, 1, int.MaxValue, cancellationToken);
		var roles = await roleRepository.GetByMaterialIdAsync(session.ShadowingMaterialId, cancellationToken);
		var attempts = await attemptRepository.GetBySessionAsync(session.Id, cancellationToken);
		var details = await attemptRepository.GetSessionDetailsAsync(
			session.Id, lines.Select(l => l.Id).ToList(), cancellationToken);

		var detailLineIds = details.Select(d => d.ShadowingRoleLineId.Value).ToHashSet();
		var cancelledRoleIds = attempts
			.Where(a => a.Status == ShadowingAttemptStatus.Cancelled && a.ShadowingSessionMember.ShadowingRoleId is not null)
			.Select(a => a.ShadowingSessionMember.ShadowingRoleId!.Value)
			.ToHashSet();

		var done = lines.Count(l => detailLineIds.Contains(l.Id.Value) || cancelledRoleIds.Contains(l.ShadowingRoleId.Value));
		var progress = lines.Count == 0 ? 0d : Math.Round((double)done / lines.Count, 4);

		var roleDtos = roles
			.OrderBy(r => r.SortOrder)
			.Select(r => new MaterialRoleSummaryDto(
				r.Id.Value, r.SpeakerId.Value, r.SortOrder, lines.Count(l => l.ShadowingRoleId == r.Id)))
			.ToList();

		return new SessionOverviewDto(
			new SessionProgressSummaryDto(lines.Count, done, progress),
			new MaterialSummaryDto(session.ShadowingMaterialId.Value, roleDtos.Count, roleDtos));
	}
}

// ---- GET ATTEMPT BY ID / LIST ATTEMPTS OF SESSION ----
public sealed record AttemptSummaryDto(
	Guid Id,
	Guid SessionId,
	Guid MemberId,
	Guid? RoleId,
	ShadowingAttemptStatus Status,
	DateTimeOffset StartedAt,
	DateTimeOffset? CompletedAt);

public sealed record MissingRoleLineDto(Guid RoleLineId, int SortOrder, string Text, TimeSpan StartTime, TimeSpan EndTime);

public sealed record AttemptViewDto(
	AttemptSummaryDto Attempt,
	PaginatedList<AttemptDetailDto> Details,
	IReadOnlyList<MissingRoleLineDto> MissingLines);

public sealed record AttemptListDto(SessionOverviewDto Overview, IReadOnlyList<AttemptSummaryDto> Attempts);

internal static class AttemptMapper
{
	public static AttemptSummaryDto ToSummary(ShadowingAttempt a) => new(
		a.Id.Value, a.ShadowingSessionId.Value, a.ShadowingSessionMember.MemberId.Value,
		a.ShadowingSessionMember.ShadowingRoleId?.Value, a.Status, a.StartedAt, a.CompletedAt);

	public static bool IsCurator(ICurrentUserContext user) => user.Role == Rakushu.Domain.Entities.Identity.Role.RoleCodes.LinguisticCurator;
}

public sealed record GetShadowingAttemptQuery(Guid SessionId, Guid AttemptId, int PageNumber, int PageSize, bool? IsSkipped)
	: IRequest<Result<AttemptViewDto>>;

internal sealed class GetShadowingAttemptValidator : AbstractValidator<GetShadowingAttemptQuery>
{
	public GetShadowingAttemptValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.AttemptId).NotEmpty();
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetShadowingAttemptHandler(
	IShadowingAttemptRepository attemptRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IStorageService storage,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingAttemptQuery, Result<AttemptViewDto>>
{
	public async Task<Result<AttemptViewDto>> Handle(GetShadowingAttemptQuery request, CancellationToken cancellationToken)
	{
		if (currentUser.UserId is null)
			return Result.Failure<AttemptViewDto>(ShadowingSessionErrors.Unauthenticated);

		var attempt = await attemptRepository.GetWithContextAsync(ShadowingAttemptId.From(request.AttemptId), cancellationToken);
		if (attempt is null || attempt.ShadowingSessionId.Value != request.SessionId)
			return Result.Failure<AttemptViewDto>(ShadowingAttemptErrors.NotFound);
		if (!AttemptMapper.IsCurator(currentUser) && attempt.ShadowingSessionMember.MemberId != currentUser.UserId)
			return Result.Failure<AttemptViewDto>(ShadowingAttemptErrors.NotOwner);

		var (details, total) = await attemptRepository.GetDetailsPagedAsync(
			attempt.Id, request.PageNumber, request.PageSize, cancellationToken, request.IsSkipped);

		var items = new List<AttemptDetailDto>();
		foreach (var d in details)
		{
			var url = string.IsNullOrWhiteSpace(d.AudioKey)
				? null
				: await storage.CreatePresignedReadUrlAsync(d.AudioKey, cancellationToken);
			items.Add(new AttemptDetailDto(
				d.Id.Value, attempt.Id.Value, d.ShadowingRoleLineId.Value,
				d.AudioKey, url, d.DurationMs, d.IsSkipped, d.SubmittedAt));
		}

		var missing = new List<MissingRoleLineDto>();
		var roleId = attempt.ShadowingSessionMember.ShadowingRoleId;
		if (attempt.Status == ShadowingAttemptStatus.Cancelled && roleId is not null)
		{
			var (lines, _) = await roleLineRepository.GetPagedAsync(
				attempt.ShadowingSession.ShadowingMaterialId, null, roleId, 1, int.MaxValue, cancellationToken);
			var done = (await attemptRepository.GetDetailsAsync(attempt.Id, cancellationToken))
				.Select(d => d.ShadowingRoleLineId).ToHashSet();
			missing.AddRange(lines
				.Where(l => !done.Contains(l.Id))
				.Select(l => new MissingRoleLineDto(l.Id.Value, l.SortOrder, l.Text, l.StartTime, l.EndTime)));
		}

		return Result.Success(new AttemptViewDto(
			AttemptMapper.ToSummary(attempt),
			PaginatedList<AttemptDetailDto>.Create(items, total, request.PageNumber, request.PageSize),
			missing));
	}
}

public sealed record GetShadowingAttemptsQuery(Guid SessionId) : IRequest<Result<AttemptListDto>>;

internal sealed class GetShadowingAttemptsValidator : AbstractValidator<GetShadowingAttemptsQuery>
{
	public GetShadowingAttemptsValidator() => RuleFor(x => x.SessionId).NotEmpty();
}

internal sealed class GetShadowingAttemptsHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingRoleRepository roleRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IShadowingAttemptRepository attemptRepository,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingAttemptsQuery, Result<AttemptListDto>>
{
	public async Task<Result<AttemptListDto>> Handle(GetShadowingAttemptsQuery request, CancellationToken cancellationToken)
	{
		if (currentUser.UserId is null)
			return Result.Failure<AttemptListDto>(ShadowingSessionErrors.Unauthenticated);

		var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
		if (session is null)
			return Result.Failure<AttemptListDto>(ShadowingSessionErrors.NotFound);

		var curator = AttemptMapper.IsCurator(currentUser);
		if (!curator && !session.Members.Any(m => m.MemberId == currentUser.UserId))
			return Result.Failure<AttemptListDto>(ShadowingSessionErrors.NotMember);

		var attempts = await attemptRepository.GetBySessionAsync(session.Id, cancellationToken);
		var visible = attempts
			.Where(a => curator || a.ShadowingSessionMember.MemberId == currentUser.UserId)
			.Select(AttemptMapper.ToSummary)
			.ToList();

		var overview = await SessionOverviewBuilder.BuildAsync(
			session, roleRepository, roleLineRepository, attemptRepository, cancellationToken);

		return Result.Success(new AttemptListDto(overview, visible));
	}
}

// GET MY ATTEMPT BY SESSION (paged details)
public sealed record MyAttemptDto(
	Guid Id,
	Guid SessionId,
	ShadowingAttemptStatus Status,
	DateTimeOffset StartedAt,
	DateTimeOffset? CompletedAt,
	PaginatedList<AttemptDetailDto> Details);

public sealed record GetMyAttemptBySessionQuery(Guid SessionId, int PageNumber, int PageSize)
	: IRequest<Result<MyAttemptDto>>;

internal sealed class GetMyAttemptBySessionValidator : AbstractValidator<GetMyAttemptBySessionQuery>
{
	public GetMyAttemptBySessionValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetMyAttemptBySessionHandler(
	IShadowingAttemptRepository attemptRepository,
	IStorageService storage,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetMyAttemptBySessionQuery, Result<MyAttemptDto>>
{
	public async Task<Result<MyAttemptDto>> Handle(GetMyAttemptBySessionQuery request, CancellationToken cancellationToken)
	{
		if (currentUser.UserId is null)
			return Result.Failure<MyAttemptDto>(ShadowingSessionErrors.Unauthenticated);

		var attempt = await attemptRepository.GetByMemberAsync(
			ShadowingSessionId.From(request.SessionId), currentUser.UserId, cancellationToken);
		if (attempt is null)
			return Result.Failure<MyAttemptDto>(ShadowingAttemptErrors.NotFound);

		var (details, total) = await attemptRepository.GetDetailsPagedAsync(
			attempt.Id, request.PageNumber, request.PageSize, cancellationToken);

		var items = new List<AttemptDetailDto>();
		foreach (var d in details)
		{
			var url = string.IsNullOrWhiteSpace(d.AudioKey)
				? null
				: await storage.CreatePresignedReadUrlAsync(d.AudioKey, cancellationToken);
			items.Add(new AttemptDetailDto(
				d.Id.Value, attempt.Id.Value, d.ShadowingRoleLineId.Value,
				d.AudioKey, url, d.DurationMs, d.IsSkipped, d.SubmittedAt));
		}

		return Result.Success(new MyAttemptDto(
			attempt.Id.Value, attempt.ShadowingSessionId.Value, attempt.Status, attempt.StartedAt, attempt.CompletedAt,
			PaginatedList<AttemptDetailDto>.Create(items, total, request.PageNumber, request.PageSize)));
	}
}

// ---- GET SESSION PROGRESS (all members, paged by role line order) ----
public sealed record SessionProgressLineDto(
	Guid RoleLineId,
	int SortOrder,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	Guid RoleId,
	Guid? MemberId,
	Guid? AttemptId,
	ShadowingAttemptStatus? AttemptStatus,
	string State,
	AttemptDetailDto? Detail);

public sealed record SessionProgressDto(SessionOverviewDto Overview, PaginatedList<SessionProgressLineDto> Lines);

public sealed record GetSessionProgressQuery(Guid SessionId, int PageNumber, int PageSize)
	: IRequest<Result<SessionProgressDto>>;

internal sealed class GetSessionProgressValidator : AbstractValidator<GetSessionProgressQuery>
{
	public GetSessionProgressValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetSessionProgressHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingRoleRepository roleRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IShadowingAttemptRepository attemptRepository,
	IStorageService storage,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetSessionProgressQuery, Result<SessionProgressDto>>
{
	public async Task<Result<SessionProgressDto>> Handle(
		GetSessionProgressQuery request, CancellationToken cancellationToken)
	{
		if (currentUser.UserId is null)
			return Result.Failure<SessionProgressDto>(ShadowingSessionErrors.Unauthenticated);

		var session = await sessionRepository.GetByIdAsync(ShadowingSessionId.From(request.SessionId), cancellationToken);
		if (session is null)
			return Result.Failure<SessionProgressDto>(ShadowingSessionErrors.NotFound);
		if (!session.Members.Any(m => m.MemberId == currentUser.UserId))
			return Result.Failure<SessionProgressDto>(ShadowingSessionErrors.NotMember);

		var (lines, total) = await roleLineRepository.GetPagedAsync(
			session.ShadowingMaterialId, null, null, request.PageNumber, request.PageSize, cancellationToken);

		var attempts = await attemptRepository.GetBySessionAsync(session.Id, cancellationToken);
		var details = await attemptRepository.GetSessionDetailsAsync(
			session.Id, lines.Select(l => l.Id).ToList(), cancellationToken);
		var detailByLine = details.ToDictionary(d => d.ShadowingRoleLineId.Value);

		var items = new List<SessionProgressLineDto>();
		foreach (var line in lines)
		{
			var owner = session.Members.FirstOrDefault(m => m.ShadowingRoleId == line.ShadowingRoleId);
			var attempt = owner is null
				? null
				: attempts.FirstOrDefault(a => a.ShadowingSessionMember.MemberId == owner.MemberId);

			AttemptDetailDto? detailDto = null;
			string state;
			if (detailByLine.TryGetValue(line.Id.Value, out var d))
			{
				var url = string.IsNullOrWhiteSpace(d.AudioKey)
					? null
					: await storage.CreatePresignedReadUrlAsync(d.AudioKey, cancellationToken);
				detailDto = new AttemptDetailDto(
					d.Id.Value, d.ShadowingAttemptId.Value, d.ShadowingRoleLineId.Value,
					d.AudioKey, url, d.DurationMs, d.IsSkipped, d.SubmittedAt);
				state = d.IsSkipped ? "Skipped" : "Completed";
			}
			else if (attempt?.Status == ShadowingAttemptStatus.Cancelled)
				state = "AttemptCancelled";
			else
				state = "Pending";

			items.Add(new SessionProgressLineDto(
				line.Id.Value, line.SortOrder, line.Text, line.StartTime, line.EndTime,
				line.ShadowingRoleId.Value, owner?.MemberId.Value, attempt?.Id.Value, attempt?.Status, state, detailDto));
		}

		var overview = await SessionOverviewBuilder.BuildAsync(
			session, roleRepository, roleLineRepository, attemptRepository, cancellationToken);
		return Result.Success(new SessionProgressDto(
			overview,
			PaginatedList<SessionProgressLineDto>.Create(items, total, request.PageNumber, request.PageSize)));
	}
}

// ---- WORKER: COMPLETE SESSIONS
public sealed record CompleteFinishedShadowingSessionsCommand(int BatchSize) : IRequest<Result<int>>;

internal sealed class CompleteFinishedShadowingSessionsHandler(
	IShadowingSessionRepository sessionRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CompleteFinishedShadowingSessionsCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CompleteFinishedShadowingSessionsCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var sessions = await sessionRepository.GetInProgressWithAttemptsAsync(request.BatchSize, cancellationToken);
			var now = DateTimeOffset.UtcNow;
			var count = 0;
			foreach (var session in sessions)
			{
				var anySubmitted = session.Attempts.Any(a => a.Status != ShadowingAttemptStatus.Cancelled);
				if (session.Finish(now, anySubmitted).IsSuccess)
					count++;
			}
			return Result.Success(count);
		}, cancellationToken);
	}
}
