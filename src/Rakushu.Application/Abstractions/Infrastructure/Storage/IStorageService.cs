using Rakushu.Application.Usecases.Storage.CompleteVideoMultipartUpload;
using Rakushu.Application.Usecases.Storage.CreateVideoMultipartUpload;
using Rakushu.Application.Usecases.Storage.CreateVideoPresignedPartUrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.Storage;

public interface IStorageService
{
	// Small Files Upload
	Task<string> CreatePresignedUploadUrlAsync(
		string key, 
		string contentType, 
		CancellationToken cancellationToken);

	// Large Files Upload
	Task<MultipartUploadDto> CreateMultipartUploadAsync(
		string key, 
		string contentType, 
		CancellationToken cancellationToken);
	Task<IReadOnlyList<PresignedPartUrlDto>> CreateMultipartPartUrlsAsync(
		string key, 
		string uploadId, 
		int partCount, 
		CancellationToken cancellationToken); 
	Task CompleteMultipartUploadAsync(
		string key, 
		string uploadId, 
		IReadOnlyList<CompletedPartDto> parts, 
		CancellationToken cancellationToken); 
	Task AbortMultipartUploadAsync(
		string key, 
		string uploadId, 
		CancellationToken cancellationToken);
	Task<string> CreatePresignedReadUrlAsync(
		string key, 
		CancellationToken cancellationToken);

	// Metadata
	Task<MediaAssetMetadataDto> GetMediaAssetMetadataAsync(
		string key, 
		CancellationToken cancellationToken);
	string GetS3MediaUri(string videoStorageKey);

	// Streaming
	Task<Stream> StreamingReadAsync(
	string key,
	CancellationToken cancellationToken);

	// Upload Text File
	Task UploadTextFileAsync(
	string key,
	string content,
	CancellationToken cancellationToken);
	Task<string?> FindTranslatedTextFileKeyAsync(
	string prefix,
	CancellationToken cancellationToken);

	Task<string> ReadTextFileAsync(
		string key,
		CancellationToken cancellationToken);
}

// DTO
public sealed record MediaAssetMetadataDto(
	string ContentType,
	string FileName,
	long FileSizeBytes
);