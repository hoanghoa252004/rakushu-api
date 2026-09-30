using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Bunsetsu;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;

namespace Rakushu.Application.Usecases.Bunsetsu.CreateBunsetsu;

public sealed record CreateBunsetsuCommand(
	Guid transcriptSegmentId,
	string text,
	int startIndex,
	int endIndex,
	int sequence
) : IRequest<Result<Guid>>;
