using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Storage.CreateAudioPresignedUrl;

internal sealed class CreateAudioPresignedUrlHandler(IStorageService storageService)
	: IRequestHandler<CreateAudioPresignedUrlCommand, Result<CreateAudioPresignedUrlResponseDto>>
{
	public async Task<Result<CreateAudioPresignedUrlResponseDto>> Handle(
		CreateAudioPresignedUrlCommand request, CancellationToken cancellationToken)
	{
		var extension = Path.GetExtension(request.FileName);

		var key = $"audios/{Guid.NewGuid():N}{extension}";

		var presignUrl = await storageService.CreatePresignedUploadUrlAsync(key, request.ContentType, cancellationToken);

		return Result.Success(new CreateAudioPresignedUrlResponseDto(key, presignUrl));
	}
}
