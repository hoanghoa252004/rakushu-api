using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.CreateVideoMultipartUpload;

internal sealed class CreateVideoMultipartUploadHandler : IRequestHandler<CreateVideoMultipartUploadCommand, Result<CreateVideoMultipartUploadResponseDto>>
{
	private readonly IStorageService _storageService;

	public CreateVideoMultipartUploadHandler(
		IStorageService storageService)
	{
		_storageService = storageService;
	}

	public async Task<Result<CreateVideoMultipartUploadResponseDto>> Handle(
		CreateVideoMultipartUploadCommand request,
		CancellationToken cancellationToken)
	{
		var key = $"media-assets/videos/{Guid.NewGuid().ToString()}";

		var result = await _storageService.CreateMultipartUploadAsync(
			key,
			request.ContentType,
			cancellationToken);

		return Result.Success(new CreateVideoMultipartUploadResponseDto(result.UploadId, key));
	}
}