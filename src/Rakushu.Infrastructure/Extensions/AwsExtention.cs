using Amazon.BedrockRuntime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.SimpleEmail;
using Amazon.TranscribeService;
using Amazon.Translate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rakushu.Application.Abstractions.Infrastructure.ASR;
using Rakushu.Application.Abstractions.Infrastructure.Email;
using Rakushu.Application.Abstractions.Infrastructure.Nlp;
using Rakushu.Application.Abstractions.Infrastructure.Rag;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Infrastructure.Translation;
using Rakushu.Infrastructure.ASR;
using Rakushu.Infrastructure.Email;
using Rakushu.Infrastructure.Extensions.Options;
using Rakushu.Infrastructure.Nlp;
using Rakushu.Infrastructure.Rag;
using Rakushu.Infrastructure.Storage;
using Rakushu.Infrastructure.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Extensions;

internal static class AwsExtention
{

	internal static IServiceCollection AddAwsServices(
			this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
	{
		// ==========  Setting DI for Aws Ses========== 
		var awsOptions = configuration.GetAWSOptions();

		services.AddDefaultAWSOptions(awsOptions);

		if (environment.IsProduction())
		{
			services.AddSingleton<IAmazonSimpleEmailService>(sp =>
			new AmazonSimpleEmailServiceClient(
				Amazon.RegionEndpoint.APSoutheast1
				));

			services.AddSingleton<IAmazonS3>(sp =>
				new AmazonS3Client(
					Amazon.RegionEndpoint.APSoutheast1
				));

			services.AddSingleton<IAmazonTranscribeService>(_ =>
				new AmazonTranscribeServiceClient(
					Amazon.RegionEndpoint.APSoutheast1));

			services.AddSingleton<IAmazonBedrockRuntime>(_ =>
				new AmazonBedrockRuntimeClient(
					Amazon.RegionEndpoint.APNortheast1));

			services.AddSingleton<IAmazonTranslate>(_ =>
				new AmazonTranslateClient(
					Amazon.RegionEndpoint.APSoutheast1));
		}
		else
		{
			var chain = new CredentialProfileStoreChain();

			if (!chain.TryGetAWSCredentials("default", out var awsCredentials))
			{
				throw new Exception("Cannot load AWS credentials");
			}

			services.AddSingleton<IAmazonSimpleEmailService>(sp =>
				new AmazonSimpleEmailServiceClient(
					awsCredentials,
					Amazon.RegionEndpoint.APSoutheast1
					));

			services.AddSingleton<IAmazonS3>(sp =>
				new AmazonS3Client(
					awsCredentials,
					Amazon.RegionEndpoint.APSoutheast1
				));

			// AWS Transcribe
			services.AddSingleton<IAmazonTranscribeService>(_ =>
				new AmazonTranscribeServiceClient(
					awsCredentials,
					Amazon.RegionEndpoint.APSoutheast1));

			// AWS Bedrock
			services.AddSingleton<IAmazonBedrockRuntime>(_ =>
				new AmazonBedrockRuntimeClient(
					awsCredentials,
					Amazon.RegionEndpoint.APNortheast1));

			// AWS Translate
			services.AddSingleton<IAmazonTranslate>(_ =>
				new AmazonTranslateClient(
					awsCredentials,
					Amazon.RegionEndpoint.APSoutheast1));
		}

		services.Configure<AwsSettings>(configuration.GetSection(AwsSettings.ConfigurationSection));

		// SES
		services.AddScoped<IEmailService, AwsSesService>();
		// S3
		services.AddScoped<IStorageService, AwsS3Service>();
		// Transcribe
		services.AddScoped<IAsrService, AwsTranscribeService>();
		services.AddScoped<IAsrOutputReader, AwsAsrOutputReader>();
		// Bedrock
		services.AddScoped<IEmbeddingService, AwsBedrockEmbeddingService>();
		// NLP Service
		services.AddHttpClient<INlpService, SelftHostingNlpService>((serviceProvider, client) =>
		{
			var configuration = serviceProvider
				.GetRequiredService<IConfiguration>();

			//var baseUrl = "http://localhost:8000";
			var baseUrl = configuration["AwsSettings:NlpBaseUrl"]
				?? throw new InvalidOperationException(
					"NlpService:BaseUrl is not configured.");

			client.BaseAddress = new Uri(baseUrl);

			client.Timeout = TimeSpan.FromMinutes(2);
		});
		// Translate
		services.AddScoped<ITranslateService, AwsTranslateService>();
		return services;
	}
}

