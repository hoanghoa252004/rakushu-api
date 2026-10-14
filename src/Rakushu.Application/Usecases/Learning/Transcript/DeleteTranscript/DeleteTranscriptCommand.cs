using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Transcript.DeleteTranscript;

public sealed record DeleteTranscriptCommand(
	Guid VideoId
) : IRequest<Result>;
