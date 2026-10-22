using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Extensions.Options;

public sealed class AwsSettings
{
	public const string ConfigurationSection = nameof(AwsSettings);
	// SES settings
	public string SenderEmail { get; set; } = string.Empty;

	// S3 settings
	public string BucketName { get; set; } = string.Empty;
	public string BucketUrl { get; set; } = string.Empty;
	public string PresignUrlExpiration { get; set; } = string.Empty;

	// Bedrock settings
	public string BedrockModelId { get; set; } = string.Empty;

	// Translate settings
	public string TranslateDataAccessRoleArn { get; set; } = string.Empty;
}
