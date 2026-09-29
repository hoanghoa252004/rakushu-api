using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.SupportedLanguage;

namespace Rakushu.Persistence.Configurations;

internal sealed class SupportedLanguageConfiguration : IEntityTypeConfiguration<SupportedLanguage>
{
	public void Configure(EntityTypeBuilder<SupportedLanguage> builder)
	{
		// Id
		builder.HasKey(sl => sl.Id);
		builder.Property(sl => sl.Id)
			.HasConversion(
				id => id.Value,
				value => SupportedLanguageId.From(value));

		// Code
		builder.Property(sl => sl.Code)
			.HasMaxLength(10)
			.IsRequired();
		builder.HasIndex(sl => sl.Code)
			.IsUnique();

		// Name
		builder.Property(sl => sl.Name)
			.HasMaxLength(100)
			.IsRequired();

		// NativeName
		builder.Property(sl => sl.NativeName)
			.HasMaxLength(100)
			.IsRequired();

		// IsActive
		builder.Property(sl => sl.IsActive)
			.IsRequired();

		// CreatedAt
		builder.Property(sl => sl.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(sl => sl.UpdatedAt)
			.IsRequired();

		
	}
}
