using FluentValidation;
using MediatR;
using Rakushu.Application.Common.Parsing;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.GetShadowingMaterials;

public sealed record GetShadowingMaterialsQuery(
	int PageNumber,
	int PageSize,
	Guid? VideoId,
	string? Status,
	int LinePageNumber = 1,
	int LinePageSize = 20)
	: IRequest<Result<PaginatedList<ShadowingMaterialDto>>>;

internal sealed class GetShadowingMaterialsValidator : AbstractValidator<GetShadowingMaterialsQuery>
{
	public GetShadowingMaterialsValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("PageNumber must be greater than 0");
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");
		RuleFor(x => x.LinePageNumber).GreaterThan(0);
		RuleFor(x => x.LinePageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetShadowingMaterialsHandler(
	IShadowingMaterialRepository repository,
	IShadowingRoleRepository roleRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IStorageService storageService,
	IVideoRepository videoRepository,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingMaterialsQuery, Result<PaginatedList<ShadowingMaterialDto>>>
{
	public async Task<Result<PaginatedList<ShadowingMaterialDto>>> Handle(
		GetShadowingMaterialsQuery request, CancellationToken cancellationToken)
	{
		var parsedStatus = EnumParser.ParseOptional<ShadowingMaterialStatus>(request.Status, "status");
		if (parsedStatus.IsFailure)
			return Result.Failure<PaginatedList<ShadowingMaterialDto>>(parsedStatus.Error);

		// Curators see all; learners only see materials they created or that belong to sessions they joined.
		var isCurator = currentUser.Role == RoleCodes.LinguisticCurator;
		if (!isCurator && currentUser.UserId is null)
			return Result.Failure<PaginatedList<ShadowingMaterialDto>>(ShadowingMaterialErrors.NotAvailable);

		var (items, total) = await repository.GetPagedAsync(
			request.PageNumber,
			request.PageSize,
			request.VideoId.HasValue ? VideoId.From(request.VideoId.Value) : null,
			parsedStatus.Value,
			isCurator ? null : currentUser.UserId,
			cancellationToken);

		var dtos = new List<ShadowingMaterialDto>();
		foreach (var item in items)
			dtos.Add(await ShadowingMaterialDto.FromEntityWithDetailsAsync(
				item, roleRepository, roleLineRepository, storageService,
				request.LinePageNumber, request.LinePageSize, cancellationToken));

		return Result.Success(PaginatedList<ShadowingMaterialDto>.Create(
			dtos,
			total,
			request.PageNumber,
			request.PageSize));
	}
}
