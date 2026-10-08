using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.CreateVideoPresignedPartUrl;

public sealed record CreateVideoPresignedPartUrlCommand(
	string Key,
	string UploadId,
	long FileSize
) : IRequest<Result<CreateVideoPresignedPartUrlResponseDto>>;

public sealed record CreateVideoPresignedPartUrlResponseDto(
	long PartSize,
	int PartCount,
	IReadOnlyList<PresignedPartUrlDto> Parts
);

public sealed record PresignedPartUrlDto(
	int PartNumber,
	string Url
	);