using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.GetDueCards;

internal sealed class GetDueCardsHandler
    : IRequestHandler<GetDueCardsQuery, Result<IReadOnlyList<SrsCardDto>>>
{
    private readonly ICurrentUserContext _currentUser;
    private readonly ISrsCardRepository _repository;
    private readonly ISystemClock _clock;

    public GetDueCardsHandler(
        ICurrentUserContext currentUser,
        ISrsCardRepository repository,
        ISystemClock clock)
    {
        _currentUser = currentUser;
        _repository = repository;
        _clock = clock;
    }

    public async Task<Result<IReadOnlyList<SrsCardDto>>> Handle(
        GetDueCardsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
        {
            return Result.Failure<IReadOnlyList<SrsCardDto>>(
                UserErrors.UnauthorizedResourceAccess);
        }

        var cards = await _repository.GetDueCardsAsync(
            userId,
            _clock.UtcNow,
            cancellationToken);

        return Result.Success<IReadOnlyList<SrsCardDto>>(
            cards.Select(SrsCardDto.FromEntity).ToList());
    }
}