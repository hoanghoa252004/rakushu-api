using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Transcript.CreateTranscript;

public sealed record ImportOfficialTranscriptCommand(
	Guid VideoId,
	string FullText,
	string StorageKey
) : IRequest<Result>;
