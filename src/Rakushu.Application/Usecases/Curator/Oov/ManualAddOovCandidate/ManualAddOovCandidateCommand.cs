using Rakushu.Domain.Common.Results;
using MediatR;
using System;

namespace Rakushu.Application.Usecases.Curator.Oov.ManualAddOovCandidate;

public sealed record ManualAddOovCandidateCommand(
	Guid OovCandidateId,
	Guid CuratorId,
	string Term,
	string Reading,
	string Pos,
	string Meaning,
	string? Comment = null
) : IRequest<Result<Guid>>;
