using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage;

internal sealed class CreatePresignedUrlHandler : IRequestHandler<CreatePresignedUrlCommand, Result<CreatePresignedUrlResponseDto>>
{
	private readonly IStorageService _storageService;

	public CreatePresignedUrlHandler(IStorageService storageService)
	{
		_storageService = storageService;
	}

	public async Task<Result<CreatePresignedUrlResponseDto>> Handle(CreatePresignedUrlCommand request, CancellationToken cancellationToken)
	{
		var result = await _storageService.CreatePresignedUrlAsync(request.ContentType, cancellationToken);

		return Result.Success(new CreatePresignedUrlResponseDto(result.Key, result.PresignUrl));
	}
}