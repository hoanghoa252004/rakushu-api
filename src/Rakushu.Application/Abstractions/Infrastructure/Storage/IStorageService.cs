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


}
