using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Storage.CompleteVideoMultipartUpload;
using Rakushu.Application.Usecases.Storage.CreateVideoMultipartUpload;
using Rakushu.Application.Usecases.Storage.CreateVideoPresignedPartUrl;
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

	public async Task<string> CreatePresignedUploadUrlAsync(
		string key, 
		string contentType, 
		CancellationToken cancellationToken)
	{
		var request = new GetPreSignedUrlRequest
		{
			BucketName = _s3Settings.BucketName,
			Key = key,
			Verb = HttpVerb.PUT,
			Expires = DateTime.UtcNow.AddMinutes(int.Parse(_s3Settings.PresignUrlExpiration)),
			ContentType = contentType
		};

		var presignUrl = await _s3.GetPreSignedURLAsync(request);

		return presignUrl;
	}

	public async Task<string> CreatePresignedReadUrlAsync(
		string key, 
		CancellationToken cancellationToken)
	{
		var request = new GetPreSignedUrlRequest
		{
			BucketName = _s3Settings.BucketName,
			Key = key,
			Verb = HttpVerb.GET,
			Expires = DateTime.UtcNow.AddMinutes(int.Parse(_s3Settings.PresignUrlExpiration))
		};

		return await _s3.GetPreSignedURLAsync(request);
	}

	public async Task<MultipartUploadDto> CreateMultipartUploadAsync(
		string key, 
		string contentType, 
		CancellationToken cancellationToken)
	{
		var request = new InitiateMultipartUploadRequest
		{
			BucketName = _s3Settings.BucketName,
			Key = key,
			ContentType = contentType
		};

		var response = await _s3.InitiateMultipartUploadAsync(
			request,
			cancellationToken);

		return new MultipartUploadDto(response.UploadId);
	}

	public Task<IReadOnlyList<PresignedPartUrlDto>>CreateMultipartPartUrlsAsync(
		string key,
		string uploadId,
		int partCount,
		CancellationToken cancellationToken)
	{
		if (partCount <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(partCount), "Part count must be greater than zero.");
		}

		var urls = new List<PresignedPartUrlDto>(partCount);

		for (var partNumber = 1; partNumber <= partCount; partNumber++)
		{
			var request = new GetPreSignedUrlRequest
			{
				BucketName = _s3Settings.BucketName,
				Key = key,
				Verb = HttpVerb.PUT,
				UploadId = uploadId,
				PartNumber = partNumber,
				Expires = DateTime.UtcNow.AddMinutes(int.Parse(_s3Settings.PresignUrlExpiration))
			};

			var url = _s3.GetPreSignedURL(request);

			urls.Add(new PresignedPartUrlDto(partNumber, url));
		}

		return Task.FromResult<IReadOnlyList<PresignedPartUrlDto>>(urls);
	}

	public async Task CompleteMultipartUploadAsync(
		string key,
		string uploadId,
		IReadOnlyList<CompletedPartDto> parts,
		CancellationToken cancellationToken)
	{
		if (parts.Count == 0)
			throw new ArgumentException("At least one completed part is required.",nameof(parts));

		var completedParts = parts
			.OrderBy(x => x.PartNumber)
			.Select(x => new PartETag
			{
				PartNumber = x.PartNumber,
				ETag = x.ETag
			}).ToList();

		var request = new CompleteMultipartUploadRequest
		{
			BucketName = _s3Settings.BucketName,
			Key = key,
			UploadId = uploadId,
			PartETags = completedParts
		};

		await _s3.CompleteMultipartUploadAsync(request,cancellationToken);
	}

	public async Task AbortMultipartUploadAsync(
		string key,
		string uploadId,
		CancellationToken cancellationToken)
	{
		var request = new AbortMultipartUploadRequest
		{
			BucketName = _s3Settings.BucketName,
			Key = key,
			UploadId = uploadId
		};

		await _s3.AbortMultipartUploadAsync(request, cancellationToken);
	}
}
