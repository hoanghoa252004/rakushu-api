namespace Rakushu.Domain.Entities.Configuration.SystemConfiguration;

/// <summary>
/// Dynamic configurations stored in database. Naming convention: {Section}_{Property}.
/// Environment-level configurations loaded once at application start (JWT secret/issuer/audience, NLP base url, ...)
/// stay in appsettings and are intentionally NOT part of this enum.
/// </summary>
public enum SystemConfigurationKey
{
	// JwtSettings
	JwtSettings_AccessTokenExpirationMinutes,
	JwtSettings_RefreshTokenExpirationDays,
	JwtSettings_EmailVerificationTokenExpirationMinutes,

	// AwsSettings
	AwsSettings_SenderEmail,
	AwsSettings_BucketName,
	AwsSettings_PresignUrlExpiration,
	AwsSettings_BedrockModelId,
	AwsSettings_BedrockChatModelId,
	AwsSettings_BedrockGuardrailId,
	AwsSettings_BedrockGuardrailVersion,
	AwsSettings_AssistantSystemPrompt,

	// AzureSettings
	AzureSettings_AzureSpeechKey,
	AzureSettings_AzureSpeechRegion,

	// VnPaySettings
	VnPaySettings_Version,
	VnPaySettings_TmnCode,
	VnPaySettings_HashSecret,
	VnPaySettings_BaseUrl,
	VnPaySettings_ReturnUrl,
	VnPaySettings_Command,
	VnPaySettings_CurrCode,
	VnPaySettings_Locale,
	VnPaySettings_OrderType,
	VnPaySettings_BankCode,
	VnPaySettings_PaymentExpiredInSeconds,
	VnPaySettings_TransactionExpiredInSeconds,
	VnPaySettings_TimeZoneId
}
