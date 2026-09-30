using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Application.Usecases.Interest.CreateInterest;

internal sealed class CreateInterestHandler : IRequestHandler<CreateInterestCommand, Result<Guid>>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateInterestHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateInterestCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var users = await _userRepository.FindAsync(u => u.Profile != null && u.Profile.Id == ProfileId.From(request.profileId), cancellationToken);
			var user = users.FirstOrDefault();
			if (user is null || user.Profile is null)
				return Result.Failure<Guid>(ProfileErrors.NotFound);

			var result = user.Profile.AddInterest(ContentCategoryId.From(request.contentCategoryId), request.priority);
			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}