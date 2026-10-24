using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.GetSpeakers;

internal sealed class GetSpeakersHandler(ISpeakerRepository repository, IStorageService storageService)
	: IRequestHandler<GetSpeakersQuery, Result<PaginatedList<SpeakerDto>>>
{
	public async Task<Result<PaginatedList<SpeakerDto>>> Handle(GetSpeakersQuery request, CancellationToken cancellationToken)
	{
		var (items, total) = await repository.GetPagedAsync(
			request.PageNumber, request.PageSize, request.SearchTerm, cancellationToken);

		var dtos = await Task.WhenAll(items.Select(s => SpeakerDto.FromEntityAsync(s, storageService, cancellationToken)));

		return Result.Success(PaginatedList<SpeakerDto>.Create(dtos, total, request.PageNumber, request.PageSize));
	}
}