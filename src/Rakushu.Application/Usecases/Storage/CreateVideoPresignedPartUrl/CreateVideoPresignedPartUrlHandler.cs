using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.CreateVideoPresignedPartUrl;

internal sealed class CreateVideoPresignedPartUrlHandler
	: IRequestHandler<
		CreateVideoPresignedPartUrlCommand,
		Result<CreateVideoPresignedPartUrlResponseDto>>
{
	private const long PartSize = 16 * 1024 * 1024;

	private readonly IStorageService _storageService;

	public CreateVideoPresignedPartUrlHandler(
		IStorageService storageService)
	{
		_storageService = storageService;
	}

	public async Task<Result<CreateVideoPresignedPartUrlResponseDto>> Handle(
		CreateVideoPresignedPartUrlCommand request,
		CancellationToken cancellationToken)
	{
		if (request.FileSize <= 0)
		{
			return Result.Failure<CreateVideoPresignedPartUrlResponseDto>(
				Error.Failure("MULTIPART_UPLOAD.FILE_SIZE_INVALID","File size must be greater than zero."));
		}

		var partCount = (int)Math.Ceiling(
			(double)request.FileSize / PartSize);

		if (partCount > 10_000)
		{
			return Result.Failure<CreateVideoPresignedPartUrlResponseDto>(
				Error.Failure("MULTIPART_UPLOAD.FILE_SIZE_TOO_LARGE","File is too large."));
		}

		var parts = await _storageService.CreateMultipartPartUrlsAsync(
				request.Key,
				request.UploadId,
				partCount,
				cancellationToken);

		return Result.Success(
			new CreateVideoPresignedPartUrlResponseDto(
				PartSize,
				partCount,
				parts));
	}
}