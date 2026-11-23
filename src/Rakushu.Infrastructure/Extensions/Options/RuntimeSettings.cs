using Rakushu.Application.Abstractions.Infrastructure.Configuration;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;

namespace Rakushu.Infrastructure.Extensions.Options;

/// <summary>Runtime (database-managed) AWS/Azure settings; every property reads the active config version by key.</summary>
public sealed class AwsRuntimeSettings(ISystemConfigurationProvider config)
{
	public string SenderEmail => config.GetString(SystemConfigurationKey.AwsSettings_SenderEmail);
	public string BucketName => config.GetString(SystemConfigurationKey.AwsSettings_BucketName);
	public string PresignUrlExpiration => config.GetInt(SystemConfigurationKey.AwsSettings_PresignUrlExpiration).ToString();
	public string BedrockModelId => config.GetString(SystemConfigurationKey.AwsSettings_BedrockModelId);
	public string BedrockChatModelId => config.GetString(SystemConfigurationKey.AwsSettings_BedrockChatModelId);
	public string BedrockGuardrailId => config.GetString(SystemConfigurationKey.AwsSettings_BedrockGuardrailId);
	public string BedrockGuardrailVersion => config.GetString(SystemConfigurationKey.AwsSettings_BedrockGuardrailVersion);
	public string AssistantSystemPrompt => config.GetString(SystemConfigurationKey.AwsSettings_AssistantSystemPrompt);
}
public sealed class AzureRuntimeSettings(ISystemConfigurationProvider config)
{
	public string AzureSpeechKey => config.GetString(SystemConfigurationKey.AzureSettings_AzureSpeechKey);
	public string AzureSpeechRegion => config.GetString(SystemConfigurationKey.AzureSettings_AzureSpeechRegion);
}
