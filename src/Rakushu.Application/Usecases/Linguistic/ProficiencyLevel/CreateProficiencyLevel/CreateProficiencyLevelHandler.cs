using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.CreateProficiencyLevel;

internal sealed class CreateProficiencyLevelHandler : IRequestHandler<CreateProficiencyLevelCommand, Result<Guid>>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateProficiencyLevelHandler(IProficiencyFrameworkRepository frameworkRepository, IUnitOfWork unitOfWork)
	{
		_frameworkRepository = frameworkRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateProficiencyLevelCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var framework = await _frameworkRepository.GetByIdAsync(ProficiencyFrameworkId.From(request.frameworkId), cancellationToken);
			if (framework is null)
				return Result.Failure<Guid>(ProficiencyFrameworkErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var result = framework.AddLevel(
				request.code,
				request.name,
				request.sortOrder,
				now,
				now,
				request.description);

			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}