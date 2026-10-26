using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Transcript.MarkTranscriptReviewed;

public sealed record MarkTranscriptReviewedCommand(
	Guid VideoId
) : IRequest<Result>;
