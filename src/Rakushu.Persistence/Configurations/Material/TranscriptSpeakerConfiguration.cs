using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations.Material;


internal sealed class TranscriptSpeakerConfiguration : IEntityTypeConfiguration<TranscriptSpeaker>
{
	public void Configure(EntityTypeBuilder<TranscriptSpeaker> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

		// Id
		builder.HasKey(ts => ts.Id);
		builder.Property(ts => ts.Id)
			.HasConversion(
				id => id.Value,
				value => TranscriptSpeakerId.From(value));

		// TranscriptId
		builder.Property(ts => ts.TranscriptId)
			.HasConversion(
				id => id.Value,
				value => TranscriptId.From(value))
			.IsRequired();
		builder.HasOne(ts => ts.Transcript)
			.WithMany(t => t.TranscriptSpeakers)
			.HasForeignKey(ts => ts.TranscriptId)
			.OnDelete(DeleteBehavior.Cascade);

		// Label
		builder.Property(ts => ts.Label)
			.HasMaxLength(30)
			.IsRequired();

		// SpeakerId
		builder.Property(ts => ts.SpeakerId)
			.HasConversion(
				id => id.Value,
				value => SpeakerId.From(value));
		builder.HasOne(ts => ts.Speaker)
			.WithMany(s => s.TranscriptSpeakers)
			.HasForeignKey(ts => ts.SpeakerId)
			.OnDelete(DeleteBehavior.Restrict);

		// Name
		builder.Property(ts => ts.Name)
			.HasMaxLength(100);

		// Description
		builder.Property(ts => ts.Description);
	}
}
