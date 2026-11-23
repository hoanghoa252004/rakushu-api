using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;
using Rakushu.Application.Abstractions.Infrastructure.Clock;

namespace Rakushu.Application.Usecases.Configuration.SystemConfiguration;

internal sealed class GetSystemConfigurationsHandler
	: IRequestHandler<GetSystemConfigurationsQuery, Result<IReadOnlyCollection<SystemConfigurationGroupDto>>>
{
	private readonly ISystemConfigurationRepository _repository;

	public GetSystemConfigurationsHandler(ISystemConfigurationRepository repository) => _repository = repository;

	public async Task<Result<IReadOnlyCollection<SystemConfigurationGroupDto>>> Handle(
		GetSystemConfigurationsQuery request, CancellationToken cancellationToken)
	{
		var items = await _repository.GetAllAsync(cancellationToken);
		IReadOnlyCollection<SystemConfigurationGroupDto> dtos = items
			.Select(SystemConfigurationMapper.ToDto)
			.GroupBy(d =>
			{
				var idx = d.ConfigKey.IndexOf('_');
				return idx > 0 ? d.ConfigKey[..idx] : d.ConfigKey;
			})
			.OrderBy(g => g.Key)
			.Select(g => new SystemConfigurationGroupDto(g.Key, g.OrderBy(c => c.ConfigKey).ToList()))
			.ToList();
		return Result.Success(dtos);
	}
}

internal sealed class GetSystemConfigurationByIdHandler
	: IRequestHandler<GetSystemConfigurationByIdQuery, Result<SystemConfigurationDto>>
{
	private readonly ISystemConfigurationRepository _repository;

	public GetSystemConfigurationByIdHandler(ISystemConfigurationRepository repository) => _repository = repository;

	public async Task<Result<SystemConfigurationDto>> Handle(
		GetSystemConfigurationByIdQuery request, CancellationToken cancellationToken)
	{
		var config = await _repository.GetByIdAsync(SystemConfigurationId.From(request.Id), cancellationToken);
		return config is null
			? Result.Failure<SystemConfigurationDto>(SystemConfigurationErrors.NotFound)
			: Result.Success(SystemConfigurationMapper.ToDto(config));
	}
}

internal sealed class UpdateSystemConfigurationHandler : IRequestHandler<UpdateSystemConfigurationCommand, Result>
{
	private readonly ISystemConfigurationRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _clock;

	public UpdateSystemConfigurationHandler(
		ISystemConfigurationRepository repository,
		IUnitOfWork unitOfWork,
		ISystemClock clock)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	public async Task<Result> Handle(UpdateSystemConfigurationCommand request, CancellationToken cancellationToken)
	{
		if (request.ConfigValue is null && request.Version is null)
			return Result.Failure(SystemConfigurationErrors.NothingToUpdate);

		if (request.ConfigValue is not null && request.Version is not null)
			return Result.Failure(SystemConfigurationErrors.BothValueAndVersion);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var config = await _repository.GetByIdAsync(SystemConfigurationId.From(request.Id), cancellationToken);
			if (config is null)
				return Result.Failure(SystemConfigurationErrors.NotFound);

			var now = _clock.UtcNow.UtcDateTime;
			return request.ConfigValue is not null
				? config.UpdateValue(request.ConfigValue, now)
				: config.ActivateVersion(request.Version!.Value, now);
		}, cancellationToken);
	}
}
