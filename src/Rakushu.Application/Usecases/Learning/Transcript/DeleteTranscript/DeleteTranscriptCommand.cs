using Entity = Rakushu.Domain.Entities.Video.Transcript.Transcript;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video.Transcript;

namespace Rakushu.Application.Usecases.Learning.Transcript.DeleteTranscript;

public sealed record DeleteTranscriptCommand(Guid TranscriptId) : IRequest<Result>;
