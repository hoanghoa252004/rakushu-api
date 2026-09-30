using Entity = Rakushu.Domain.Entities.Video.Transcript.Transcript;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video.Transcript;

namespace Rakushu.Application.Usecases.Transcript.CreateTranscript;

public sealed record CreateTranscriptCommand(
	Guid videoId,
	string fullText
) : IRequest<Result<Guid>>;
