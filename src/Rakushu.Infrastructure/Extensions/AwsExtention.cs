using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.SimpleEmail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rakushu.Application.Abstractions.Infrastructure.Email;
using Rakushu.Infrastructure.Email;
using Rakushu.Infrastructure.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Extensions;

internal static class AwsExtention
{

	internal static IServiceCollection AddAwsServices(
			this IServiceCollection services, IConfiguration configuration)
	{
		// ==========  Setting DI for Aws Ses========== 
		var awsOptions = configuration.GetAWSOptions();

		var chain = new CredentialProfileStoreChain();

		if (!chain.TryGetAWSCredentials("default", out var awsCredentials))
		{
			throw new Exception("Cannot load AWS credentials");
		}

		services.AddDefaultAWSOptions(awsOptions);

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
		
		services.Configure<AwsSettings>(configuration.GetSection(AwsSettings.ConfigurationSection));

		// SES
		services.AddScoped<IEmailService, AwsSesService>();
		return services;
	}
}

