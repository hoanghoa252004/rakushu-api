using Entity = Rakushu.Domain.Entities.Video.Transcript.Transcript;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Transcript.GetTranscriptById;

public sealed record GetTranscriptByIdQuery(Guid TranscriptId) : IRequest<Result<TranscriptDto>>;
