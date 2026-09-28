using Rakushu.Application.Usecases.Curator.Oov.Common;
using Rakushu.Domain.Common.Results;
using MediatR;
using System;

namespace Rakushu.Application.Usecases.Curator.Oov.GetOovCandidateById;

public sealed record GetOovCandidateByIdQuery(Guid Id) : IRequest<Result<OovCandidateDetailDto>>;
