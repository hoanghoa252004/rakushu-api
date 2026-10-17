using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.GetActiveCards;

internal sealed class GetActiveCardsHandler
    : IRequestHandler<GetActiveCardsQuery, Result<IReadOnlyList<SrsCardDto>>>
{
    private readonly ICurrentUserContext _currentUser;
    private readonly ISrsCardRepository _repository;

    public GetActiveCardsHandler(
        ICurrentUserContext currentUser,
        ISrsCardRepository repository)
    {
        _currentUser = currentUser;
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<SrsCardDto>>> Handle(
        GetActiveCardsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
        {
            return Result.Failure<IReadOnlyList<SrsCardDto>>(
                UserErrors.UnauthorizedResourceAccess);
        }

        var cards = await _repository.GetAllActiveCardsAsync(userId, cancellationToken);

        return Result.Success<IReadOnlyList<SrsCardDto>>(
            cards.Select(SrsCardDto.FromEntity).ToList());
    }
}
