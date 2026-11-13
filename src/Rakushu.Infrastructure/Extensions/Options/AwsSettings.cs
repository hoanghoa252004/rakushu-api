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
	// MediaConvert settings
	public string MediaConvertEndpoint { get; set; } = string.Empty;
	public string MediaConvertRoleArn { get; set; } = string.Empty;

	// NLP settings
	public string NlpBaseUrl { get; set; } = string.Empty;

	// Pitch assessment settings
	public string PitchAssessmentServiceBaseUrl { get; set; } = string.Empty;

	// Azure Speech settings
	public string AzureSpeechKey { get; set; } = string.Empty;
	public string AzureSpeechRegion { get; set; } = string.Empty;

}
