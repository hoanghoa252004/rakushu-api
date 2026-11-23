using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.GetMeaningMediaContexts;

public sealed record GetMeaningMediaContextsQuery(
	Guid MeaningId,
	int MaxCount = 10) : IRequest<Result<IReadOnlyList<MeaningMediaContextDto>>>;
