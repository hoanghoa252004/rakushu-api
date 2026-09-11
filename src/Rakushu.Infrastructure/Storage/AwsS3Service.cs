using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Infrastructure.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Storage;

internal class AwsS3Service : IStorageService
{
	private readonly IAmazonS3 _s3;
	private readonly AwsSettings _s3Settings;

	public AwsS3Service(IAmazonS3 s3, IOptions<AwsSettings> options)
	{
		_s3 = s3;
		_s3Settings = options.Value;
	}

	public async Task<string> CreatePresignedUrlAsync(string contentType, CancellationToken cancellationToken)
	{
		var request = new GetPreSignedUrlRequest
		{
			BucketName = _s3Settings.BucketName,
			Key = Guid.NewGuid().ToString(),
			Verb = HttpVerb.PUT,
			Expires = DateTime.UtcNow.AddMinutes(5),
			ContentType = contentType
		};

		return await _s3.GetPreSignedURLAsync(request);
	}
}
