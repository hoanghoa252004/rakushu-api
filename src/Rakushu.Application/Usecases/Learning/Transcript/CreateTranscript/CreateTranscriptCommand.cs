using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Transcript.CreateTranscript;

public sealed record CreateTranscriptCommand(
	Guid VideoId,
	string FullText,
	string StorageKey
) : IRequest<Result>;
