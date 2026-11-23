using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Abstractions.Infrastructure.Configuration;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;

namespace Rakushu.Persistence.Repositories;

internal sealed class SystemConfigurationProvider : ISystemConfigurationProvider
{
	private readonly RakushuDbContext _context;

	public SystemConfigurationProvider(RakushuDbContext context)
	{
		_context = context;
	}

	public string GetString(SystemConfigurationKey key) => GetValue(key);

	public int GetInt(SystemConfigurationKey key)
		=> int.Parse(GetValue(key).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture);

	public bool GetBool(SystemConfigurationKey key) => bool.Parse(GetValue(key).Trim());

	public decimal GetDecimal(SystemConfigurationKey key)
		=> decimal.Parse(GetValue(key).Trim(), NumberStyles.Number, CultureInfo.InvariantCulture);

	public Task<string> GetStringAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default)
		=> GetValueAsync(key, cancellationToken);

	public async Task<int> GetIntAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default)
		=> int.Parse((await GetValueAsync(key, cancellationToken)).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture);

	public async Task<bool> GetBoolAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default)
		=> bool.Parse((await GetValueAsync(key, cancellationToken)).Trim());

	public async Task<decimal> GetDecimalAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default)
		=> decimal.Parse((await GetValueAsync(key, cancellationToken)).Trim(), NumberStyles.Number, CultureInfo.InvariantCulture);

	private string GetValue(SystemConfigurationKey key)
	{
		var value = _context.SystemConfigurations
			.AsNoTracking()
			.Where(c => c.ConfigKey == key)
			.Select(c => c.ConfigValue)
			.SingleOrDefault();

		return value ?? throw new InvalidOperationException($"System configuration '{key}' was not found.");
	}

	private async Task<string> GetValueAsync(SystemConfigurationKey key, CancellationToken cancellationToken)
	{
		var value = await _context.SystemConfigurations
			.AsNoTracking()
			.Where(c => c.ConfigKey == key)
			.Select(c => c.ConfigValue)
			.SingleOrDefaultAsync(cancellationToken);

		return value ?? throw new InvalidOperationException($"System configuration '{key}' was not found.");
	}
}
