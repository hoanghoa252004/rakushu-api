using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetVocabularySuggestions;

public sealed record GetVocabularySuggestionsQuery(
	int Limit = 12) : IRequest<Result<IReadOnlyList<EncounteredVocabularySuggestionDto>>>;
