using Entity = Rakushu.Domain.Entities.Video.Transcript.Transcript;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Learning.Transcript;

namespace Rakushu.Application.Usecases.Learning.Transcript.GetTranscriptById;

public sealed record GetTranscriptByIdQuery(Guid TranscriptId) : IRequest<Result<TranscriptDto>>;
