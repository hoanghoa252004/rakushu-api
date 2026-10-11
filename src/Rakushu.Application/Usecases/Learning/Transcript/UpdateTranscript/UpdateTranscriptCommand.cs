using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Transcript.UpdateTranscript;

public sealed record UpdateTranscriptCommand(
	Guid VideoId,
	string FullText,
	string StorageKey
) : IRequest<Result>;
