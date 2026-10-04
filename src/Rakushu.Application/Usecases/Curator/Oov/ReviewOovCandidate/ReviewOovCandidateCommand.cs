using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.CuratorReview;
using System;

namespace Rakushu.Application.Usecases.Curator.Oov.ReviewOovCandidate;

public sealed record ReviewOovCandidateCommand(
	Guid OovCandidateId,
	Guid CuratorId,
	CuratorDecision Decision,
	string? EditedTerm = null,
	string? EditedReading = null,
	string? EditedPos = null,
	string? EditedMeaning = null,
	string? Comment = null
) : IRequest<Result<Guid>>;
