using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Configuration.SystemConfiguration;

public sealed record SystemConfigurationVersionDto(int Version, string ConfigValue, bool IsActive, DateTime CreatedAt);

public sealed record SystemConfigurationDto(
	Guid Id,
	string ConfigKey,
	string DataType,
	string ConfigValue,
	string? Description,
	int VersionInUsed,
	DateTime UpdatedAt,
	IReadOnlyCollection<SystemConfigurationVersionDto> Histories);

public sealed record SystemConfigurationGroupDto(string GroupName, IReadOnlyCollection<SystemConfigurationDto> Configurations);

public sealed record GetSystemConfigurationsQuery : IRequest<Result<IReadOnlyCollection<SystemConfigurationGroupDto>>>;

public sealed record GetSystemConfigurationByIdQuery(Guid Id) : IRequest<Result<SystemConfigurationDto>>;

public sealed record UpdateSystemConfigurationCommand(Guid Id, string? ConfigValue, int? Version) : IRequest<Result>;

internal static class SystemConfigurationMapper
{
	internal static SystemConfigurationDto ToDto(Rakushu.Domain.Entities.Configuration.SystemConfiguration.SystemConfiguration c)
		=> new(
			c.Id.Value,
			c.ConfigKey.ToString(),
			c.DataType.ToString(),
			c.ConfigValue,
			c.Description,
			c.VersionInUsed,
			c.UpdatedAt,
			c.Histories
				.OrderByDescending(h => h.Version)
				.Select(h => new SystemConfigurationVersionDto(h.Version, h.ConfigValue, h.IsActive, h.CreatedAt))
				.ToList());
}
