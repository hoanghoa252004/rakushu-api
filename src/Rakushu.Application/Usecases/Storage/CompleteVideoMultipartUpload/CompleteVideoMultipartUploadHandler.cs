using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.CompleteVideoMultipartUpload;


internal sealed class CompleteVideoMultipartUploadHandler : IRequestHandler<CompleteVideoMultipartUploadCommand, Result>
{
	private readonly IStorageService _storageService;

	public CompleteVideoMultipartUploadHandler(
		IStorageService storageService)
	{
		_storageService = storageService;
	}

	public async Task<Result> Handle(
		CompleteVideoMultipartUploadCommand request,
		CancellationToken cancellationToken)
	{
		if (request.Parts.Count == 0)
		{
			return Result.Failure(Error.Failure("COMPLETE_MULTIPART_UPLOAD.NO_PARTS","At least one uploaded part is required."));
		}

		await _storageService.CompleteMultipartUploadAsync(
			request.Key,
			request.UploadId,
			request.Parts,
			cancellationToken);

		return Result.Success();
	}
}