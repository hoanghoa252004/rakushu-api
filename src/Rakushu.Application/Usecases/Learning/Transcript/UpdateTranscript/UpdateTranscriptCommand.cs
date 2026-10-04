using Entity = Rakushu.Domain.Entities.Video.Transcript.Transcript;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video.Transcript;

namespace Rakushu.Application.Usecases.Learning.Transcript.UpdateTranscript;

public sealed record UpdateTranscriptCommand(
	Guid TranscriptId,
	Guid videoId,
	string fullText
) : IRequest<Result>;
