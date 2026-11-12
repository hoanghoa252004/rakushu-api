using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Storage.CreateAudioPresignedUrl;

public sealed record CreateAudioPresignedUrlCommand(string ContentType, string FileName) : IRequest<Result<CreateAudioPresignedUrlResponseDto>>;

public sealed record CreateAudioPresignedUrlResponseDto(
	string Key,
	string PresignUrl
	);
