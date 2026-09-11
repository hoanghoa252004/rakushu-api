using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Extensions.Options;

public sealed class AwsSettings
{
	public const string ConfigurationSection = nameof(AwsSettings);
	public string SenderEmail { get; set; } = string.Empty;
	public string BucketName { get; set; } = string.Empty;
	public string BucketUrl { get; set; } = string.Empty;
}
