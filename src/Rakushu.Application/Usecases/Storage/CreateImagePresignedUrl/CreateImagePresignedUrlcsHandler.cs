using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.CreateImagePresignedUrl;

internal sealed class CreateImagePresignedUrlcsHandler : IRequestHandler<CreateImagePresignedUrlCommand, Result<CreateImagePresignedUrlResponseDto>>
{

	// SERVICES
	private readonly IStorageService _storageService;

	public CreateImagePresignedUrlcsHandler(
		IStorageService storageService
		)
	{
		_storageService = storageService;
	}

	public async Task<Result<CreateImagePresignedUrlResponseDto>> Handle(CreateImagePresignedUrlCommand request, CancellationToken cancellationToken)
	{
		var key = $"images/{Guid.NewGuid().ToString()}";

		var presignUrl = await _storageService.CreatePresignedUploadUrlAsync(key, request.ContentType, cancellationToken);

		return Result.Success(new CreateImagePresignedUrlResponseDto(key, presignUrl));
	}
}