using FluentValidation;
using MediatR;
using Rakushu.Application.Common.Parsing;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingSessions.GetShadowingSessions;

public sealed record GetShadowingSessionsQuery(
	string? Status,
	string? Visibility,
	Guid? HostId,
	int PageNumber,
	int PageSize) : IRequest<Result<PaginatedList<ShadowingSessionDto>>>;

public sealed record GetMyShadowingSessionsQuery(
	string? Status,
	string? Visibility,
	Guid? HostId,
	int PageNumber,
	int PageSize) : IRequest<Result<PaginatedList<ShadowingSessionDto>>>;

internal sealed class GetMyShadowingSessionsValidator : AbstractValidator<GetMyShadowingSessionsQuery>
{
	public GetMyShadowingSessionsValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetMyShadowingSessionsHandler(
	IShadowingSessionRepository repository,
	IStorageService storage,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetMyShadowingSessionsQuery, Result<PaginatedList<ShadowingSessionDto>>>
{
	public async Task<Result<PaginatedList<ShadowingSessionDto>>> Handle(
		GetMyShadowingSessionsQuery request, CancellationToken cancellationToken)
	{
		if (currentUser.UserId is null)
			return Result.Failure<PaginatedList<ShadowingSessionDto>>(ShadowingSessionErrors.Unauthenticated);
		var parsedStatus = EnumParser.ParseOptional<ShadowingSessionStatus>(request.Status, "status");
		if (parsedStatus.IsFailure)
			return Result.Failure<PaginatedList<ShadowingSessionDto>>(parsedStatus.Error);
		var parsedVisibility = EnumParser.ParseOptional<ShadowingSessionVisibility>(request.Visibility, "visibility");
		if (parsedVisibility.IsFailure)
			return Result.Failure<PaginatedList<ShadowingSessionDto>>(parsedVisibility.Error);

		var hostId = request.HostId.HasValue ? UserId.From(request.HostId.Value) : null;
		var filter = new ShadowingSessionFilter(
			MemberId: currentUser.UserId, HostId: hostId, Status: parsedStatus.Value, Visibility: parsedVisibility.Value);

		var (items, total) = await repository.GetPagedAsync(filter, request.PageNumber, request.PageSize, cancellationToken);
		var avatarUrls = await SessionAvatarResolver.ResolveAsync(items, storage, cancellationToken);
		return Result.Success(PaginatedList<ShadowingSessionDto>.Create(
			items.Select(s => ShadowingSessionDto.FromEntity(s, avatarUrls)).ToList(),
			total, request.PageNumber, request.PageSize));
	}
}

internal sealed class GetShadowingSessionsValidator : AbstractValidator<GetShadowingSessionsQuery>
{
	public GetShadowingSessionsValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetShadowingSessionsHandler(
	IShadowingSessionRepository repository,
	IStorageService storage,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingSessionsQuery, Result<PaginatedList<ShadowingSessionDto>>>
{
	public async Task<Result<PaginatedList<ShadowingSessionDto>>> Handle(
		GetShadowingSessionsQuery request, CancellationToken cancellationToken)
	{
		var parsedStatus = EnumParser.ParseOptional<ShadowingSessionStatus>(request.Status, "status");
		if (parsedStatus.IsFailure)
			return Result.Failure<PaginatedList<ShadowingSessionDto>>(parsedStatus.Error);
		var parsedVisibility = EnumParser.ParseOptional<ShadowingSessionVisibility>(request.Visibility, "visibility");
		if (parsedVisibility.IsFailure)
			return Result.Failure<PaginatedList<ShadowingSessionDto>>(parsedVisibility.Error);

		var hostId = request.HostId.HasValue ? UserId.From(request.HostId.Value) : null;
		var isCurator = currentUser.Role == RoleCodes.LinguisticCurator;
		if (!isCurator && parsedVisibility.Value == ShadowingSessionVisibility.Private)
			return Result.Failure<PaginatedList<ShadowingSessionDto>>(ShadowingSessionErrors.NotPublic);

		var filter = new ShadowingSessionFilter(
			HostId: hostId,
			Status: parsedStatus.Value,
			Visibility: isCurator ? parsedVisibility.Value : ShadowingSessionVisibility.Public);

		var (items, total) = await repository.GetPagedAsync(filter, request.PageNumber, request.PageSize, cancellationToken);

		var avatarUrls = await SessionAvatarResolver.ResolveAsync(items, storage, cancellationToken);

		return Result.Success(PaginatedList<ShadowingSessionDto>.Create(
			items.Select(s => ShadowingSessionDto.FromEntity(s, avatarUrls)).ToList(),
			total,
			request.PageNumber,
			request.PageSize));
	}
}
