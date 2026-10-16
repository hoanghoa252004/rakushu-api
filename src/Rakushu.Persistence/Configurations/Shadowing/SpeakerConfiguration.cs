using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class SpeakerConfiguration : IEntityTypeConfiguration<Speaker>
{
	public void Configure(EntityTypeBuilder<Speaker> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(ts => ts.Id);
		builder.Property(ts => ts.Id)
			.HasConversion(
				id => id.Value,
				value => SpeakerId.From(value));

		// Code
		builder.Property(pl => pl.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(u => u.Code)
			.IsUnique();

		// Name
		builder.Property(ts => ts.Name)
			.HasMaxLength(100)
			.IsRequired();

		// JapaneseName
		builder.Property(ts => ts.JapaneseName)
			.HasMaxLength(100)
			.IsRequired();

		// AvatarKey
		builder.Property(ts => ts.AvatarKey)
			.HasMaxLength(500);

		// Description
		builder.Property(ts => ts.Description);

		// ThemeColor
		builder.Property(ts => ts.ThemeColor)
			.HasMaxLength(10)
			.IsRequired();

		// CreatedAt
		builder.Property(u => u.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(u => u.UpdatedAt)
			.IsRequired();
	}
}
