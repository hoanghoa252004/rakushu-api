using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.UpdateProficiencyLevel;

internal sealed class UpdateProficiencyLevelHandler : IRequestHandler<UpdateProficiencyLevelCommand, Result>
{
	private readonly IProficiencyLevelRepository _proficiencyLevelRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public UpdateProficiencyLevelHandler(IUnitOfWork unitOfWork, IProficiencyLevelRepository proficiencyLevelRepository, ISystemClock systemClock)
	{
		_unitOfWork = unitOfWork;
		_proficiencyLevelRepository = proficiencyLevelRepository;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(UpdateProficiencyLevelCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var levelId = ProficiencyLevelId.From(request.ProficiencyLevelId);

			var level =  await _proficiencyLevelRepository.GetByIdAsync(levelId);

			if(level is null)
			{
				return Result.Failure(ProficiencyLevelErrors.NotFound);
			}

			level.Update(
				name: request.Name,
				japaneseName: request.JapaneseName,
				sortOrder: request.SortOrder,
				isActive: request.IsActive,
				updatedAt: _systemClock.UtcNow,
				description: request.Description
			);

			return Result.Success();
		}, cancellationToken);
	}
}