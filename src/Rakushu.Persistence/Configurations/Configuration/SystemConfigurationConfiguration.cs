using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;
using Rakushu.Domain.Entities.Configuration.SystemConfigurationHistory;

namespace Rakushu.Persistence.Configurations.Configuration;

internal sealed class SystemConfigurationConfiguration : IEntityTypeConfiguration<SystemConfiguration>
{
	public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
	{
		builder.Metadata.SetSchema("configuration");

		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id)
			.HasConversion(id => id.Value, value => SystemConfigurationId.From(value));

		builder.Property(x => x.ConfigKey)
			.HasConversion<string>()
			.HasMaxLength(100)
			.IsRequired();
		builder.HasIndex(x => x.ConfigKey).IsUnique();

		builder.Property(x => x.DataType)
			.HasConversion<string>()
			.HasMaxLength(30)
			.IsRequired();

		builder.Property(x => x.ConfigValue).IsRequired();
		builder.Property(x => x.Description);
		builder.Property(x => x.VersionInUsed).IsRequired();
		builder.Property(x => x.UpdatedAt).IsRequired();

		builder.HasMany(x => x.Histories)
			.WithOne(h => h.Configuration)
			.HasForeignKey(h => h.ConfigurationId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Navigation(x => x.Histories).UsePropertyAccessMode(PropertyAccessMode.Field);
	}
}

internal sealed class SystemConfigurationHistoryConfiguration : IEntityTypeConfiguration<SystemConfigurationHistory>
{
	public void Configure(EntityTypeBuilder<SystemConfigurationHistory> builder)
	{
		builder.Metadata.SetSchema("configuration");

		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id)
			.HasConversion(id => id.Value, value => SystemConfigurationHistoryId.From(value));

		builder.Property(x => x.ConfigurationId)
			.HasConversion(id => id.Value, value => SystemConfigurationId.From(value));

		builder.Property(x => x.Version).IsRequired();
		builder.Property(x => x.ConfigValue).IsRequired();
		builder.Property(x => x.IsActive).IsRequired();
		builder.Property(x => x.CreatedAt).IsRequired();

		builder.HasIndex(x => new { x.ConfigurationId, x.Version }).IsUnique();
		// At most one active version per configuration
		builder.HasIndex(x => x.ConfigurationId)
			.HasDatabaseName("ux_system_configuration_histories_active")
			.HasFilter("is_active = true")
			.IsUnique();
	}
}
