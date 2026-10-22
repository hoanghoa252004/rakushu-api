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

	public async Task<MediaAssetMetadataDto> GetMediaAssetMetadataAsync(string key, CancellationToken cancellationToken)
	{
		var response = await _s3.GetObjectMetadataAsync(
			new GetObjectMetadataRequest
			{
				BucketName = _s3Settings.BucketName,
				Key = key
			},
			cancellationToken);

		return new MediaAssetMetadataDto
		(
			response.Headers.ContentType,
			Path.GetFileName(key),
			response.ContentLength
		);
	}

	public async Task<Stream> StreamingReadAsync(string key, CancellationToken cancellationToken)
	{
		var response = await _s3.GetObjectAsync(
		new GetObjectRequest
		{
			BucketName = _s3Settings.BucketName,
			Key = key
		},
		cancellationToken);

		return new S3ObjectReadStream(response);
	}

	public string GetS3MediaUri(string videoStorageKey)
	{
		return $"s3://{_s3Settings.BucketName}/{videoStorageKey}";
	}

	public async Task UploadTextFileAsync(
	string key,
	string content,
	CancellationToken cancellationToken)
	{
		var bytes = Encoding.UTF8.GetBytes(content);

		await using var stream = new MemoryStream(bytes);

		var request = new PutObjectRequest
		{
			BucketName = _s3Settings.BucketName,
			Key = key,
			InputStream = stream,
			ContentType = "text/plain; charset=utf-8"
		};

		await _s3.PutObjectAsync(request, cancellationToken);
	}

	public async Task<string?> FindTranslatedTextFileKeyAsync(
	string prefix,
	CancellationToken cancellationToken)
	{
		// Chuẩn hóa prefix để đảm bảo chỉ tìm trong thư mục output
		// của transcript tương ứng.
		prefix = prefix.TrimStart('/');

		if (!prefix.EndsWith('/'))
		{
			prefix += "/";
		}

		string? translatedFileKey = null;
		string? continuationToken = null;

		do
		{
			var request = new ListObjectsV2Request
			{
				BucketName = _s3Settings.BucketName,
				Prefix = prefix,
				ContinuationToken = continuationToken
			};

			var response = await _s3.ListObjectsV2Async(
				request,
				cancellationToken);

			var textFiles = response.S3Objects
				.Where(obj =>
					obj.Key.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
				.Where(obj =>
					!obj.Key.EndsWith("manifest.txt", StringComparison.OrdinalIgnoreCase))
				.ToList();

			if (textFiles.Count > 1)
			{
				throw new InvalidOperationException(
					$"Tìm thấy nhiều file dịch trong S3 prefix '{prefix}'. " +
					"Không thể xác định file output cần đọc.");
			}

			if (textFiles.Count == 1)
			{
				if (translatedFileKey is not null)
				{
					throw new InvalidOperationException(
						$"Tìm thấy nhiều file dịch trong S3 prefix '{prefix}'. " +
						"Không thể xác định file output cần đọc.");
				}

				translatedFileKey = textFiles[0].Key;
			}

			continuationToken = response.IsTruncated == true
				? response.NextContinuationToken
				: null;
		}
		while (continuationToken is not null);

		return translatedFileKey;
	}

	public async Task<string> ReadTextFileAsync(
		string key,
		CancellationToken cancellationToken)
	{
		await using var stream = await StreamingReadAsync(
			key,
			cancellationToken);

		using var reader = new StreamReader(
			stream,
			Encoding.UTF8,
			detectEncodingFromByteOrderMarks: true);

		return await reader.ReadToEndAsync(cancellationToken);
	}
}
