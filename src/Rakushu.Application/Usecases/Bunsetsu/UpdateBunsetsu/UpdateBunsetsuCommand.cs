using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Bunsetsu;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;

namespace Rakushu.Application.Usecases.Bunsetsu.UpdateBunsetsu;

public sealed record UpdateBunsetsuCommand(
	Guid BunsetsuId,
	Guid transcriptSegmentId,
	string text,
	int startIndex,
	int endIndex,
	int sequence
) : IRequest<Result>;
