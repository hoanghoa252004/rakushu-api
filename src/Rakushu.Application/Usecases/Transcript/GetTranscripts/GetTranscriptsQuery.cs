using Entity = Rakushu.Domain.Entities.Video.Transcript.Transcript;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Transcript.GetTranscripts;

public sealed record GetTranscriptsQuery : IRequest<Result<IReadOnlyCollection<TranscriptDto>>>;
