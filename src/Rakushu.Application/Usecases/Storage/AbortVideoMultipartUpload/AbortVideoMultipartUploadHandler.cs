using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.AbortVideoMultipartUpload;

internal sealed class AbortVideoMultipartUploadHandler
	: IRequestHandler<AbortVideoMultipartUploadCommand, Result>
{
	private readonly IStorageService _storageService;

	public AbortVideoMultipartUploadHandler(
		IStorageService storageService)
	{
		_storageService = storageService;
	}

	public async Task<Result> Handle(
		AbortVideoMultipartUploadCommand request,
		CancellationToken cancellationToken)
	{
		await _storageService.AbortMultipartUploadAsync(
			request.Key,
			request.UploadId,
			cancellationToken);

		return Result.Success();
	}
}