using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.CreateImagePresignedUrl;

internal sealed class CreateImagePresignedUrlcsHandler : IRequestHandler<CreateImagePresignedUrlCommand, Result<PresignedUrlResponseDto>>
{

	// SERVICES
	private readonly IStorageService _storageService;

	public CreateImagePresignedUrlcsHandler(
		IStorageService storageService
		)
	{
		_storageService = storageService;
	}

	public async Task<Result<PresignedUrlResponseDto>> Handle(CreateImagePresignedUrlCommand request, CancellationToken cancellationToken)
	{
		var key = $"images/{Guid.NewGuid().ToString()}";

		var presignUrl = await _storageService.CreateImagePresignedUrlAsync(key, request.ContentType, cancellationToken);

		return Result.Success(new PresignedUrlResponseDto(key, presignUrl));
	}
}