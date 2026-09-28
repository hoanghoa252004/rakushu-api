using Rakushu.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;

namespace Rakushu.Application.Usecases.Curator.Oov.IngestOovCandidates;

public sealed record IngestOovItem(
	string Term,
	Guid? TokenId = null,
	string? TentativeReading = null,
	string? TentativePos = null,
	string? SuggestedMeaning = null,
	string? ContextSnippet = null,
	double ConfidenceScore = 0.0
);

public sealed record IngestOovCandidatesCommand(
	List<IngestOovItem> Items
) : IRequest<Result<int>>;
