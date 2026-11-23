using Amazon;
using Amazon.BedrockRuntime;
using Amazon.Runtime;

namespace Rakushu.Infrastructure.Ai;

// Dedicated Bedrock runtime client for AI chat (separate region from embedding).
internal interface IAmazonBedrockRuntimeAI : IAmazonBedrockRuntime
{
}

internal sealed class AmazonBedrockRuntimeAIClient : AmazonBedrockRuntimeClient, IAmazonBedrockRuntimeAI
{
	public AmazonBedrockRuntimeAIClient(RegionEndpoint region)
		: base(region)
	{
	}

	public AmazonBedrockRuntimeAIClient(AWSCredentials credentials, RegionEndpoint region)
		: base(credentials, region)
	{
	}
}
