using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardItemRating;

public sealed record SubmitFlashcardItemRatingCommand(
	Guid SessionId,
	Guid ItemId,
	string Rating) : IRequest<Result>;
