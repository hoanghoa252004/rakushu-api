using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;

namespace Rakushu.Persistence.Configurations.Processing;

internal sealed class ProcessingJobLogConfiguration : IEntityTypeConfiguration<ProcessingJobLog>
{
	public void Configure(EntityTypeBuilder<ProcessingJobLog> builder)
	{
		builder.ToTable("processing_job_logs", "processing");

		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id)
			.HasConversion(id => id.Value, value => ProcessingJobLogId.From(value));

		builder.Property(x => x.JobType)
			.HasConversion<string>()
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(x => x.TargetType)
			.HasConversion<string>()
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(x => x.TargetId).IsRequired();
		builder.Property(x => x.Sequence).IsRequired();
		builder.Property(x => x.ServiceName).HasMaxLength(100).IsRequired();
		builder.Property(x => x.JobLabel).HasMaxLength(255);
		builder.Property(x => x.Input);
		builder.Property(x => x.Output);
		builder.Property(x => x.IsSuccess).IsRequired().HasDefaultValue(false);
		builder.Property(x => x.IsFailed).IsRequired().HasDefaultValue(false);
		builder.Property(x => x.StartAt).IsRequired();
		builder.Property(x => x.EndAt);

		builder.Ignore(x => x.IsFinished);

		builder.HasIndex(x => new { x.TargetId, x.JobType, x.StartAt });
		builder.HasIndex(x => new { x.TargetType, x.TargetId, x.Sequence });
		builder.HasIndex(x => x.JobLabel);
	}
}
