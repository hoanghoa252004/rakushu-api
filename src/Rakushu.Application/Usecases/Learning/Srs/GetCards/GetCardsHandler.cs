using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.GetCards;

internal sealed class GetCardsHandler
	: IRequestHandler<GetCardsQuery, Result<IReadOnlyList<SrsCardDto>>>
{
	private readonly ICurrentUserContext _currentUser;
	private readonly ISrsCardRepository _repository;

	public GetCardsHandler(
		ICurrentUserContext currentUser,
		ISrsCardRepository repository)
	{
		_currentUser = currentUser;
		_repository = repository;
	}

	public async Task<Result<IReadOnlyList<SrsCardDto>>> Handle(
		GetCardsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure<IReadOnlyList<SrsCardDto>>(
				UserErrors.UnauthorizedResourceAccess);
		}

		var cards = await _repository.GetCardsAsync(userId, cancellationToken);

		return Result.Success<IReadOnlyList<SrsCardDto>>(
			cards.Select(SrsCardDto.FromEntity).ToList());
	}
}
