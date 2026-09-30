using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyFramework.CreateProficiencyFramework;

internal sealed class CreateProficiencyFrameworkHandler : IRequestHandler<CreateProficiencyFrameworkCommand, Result<Guid>>
{
	private readonly IProficiencyFrameworkRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateProficiencyFrameworkHandler(IProficiencyFrameworkRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateProficiencyFrameworkCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = DateTimeOffset.UtcNow;
			var frameworkResult = Domain.Entities.ProficiencyFramework.ProficiencyFramework.Create(
				request.code,
				request.name,
				request.isActive,
				now,
				now,
				request.description);

			if (frameworkResult.IsFailure)
				return Result.Failure<Guid>(frameworkResult.Error);

			_repository.Add(frameworkResult.Value);
			return Result.Success(frameworkResult.Value.Id.Value);
		}, cancellationToken);
	}
}