using Entity = Rakushu.Domain.Entities.Material.Bunsetsu.Bunsetsu;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Bunsetsu.CreateBunsetsu;

public sealed record CreateBunsetsuCommand(
	Guid transcriptSegmentId,
	string text,
	int startIndex,
	int endIndex,
	int sequence
) : IRequest<Result<Guid>>;
