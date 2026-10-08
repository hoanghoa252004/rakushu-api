using MediatR;
using Rakushu.Application.Usecases.Storage.CreateVideoPresignedPartUrl;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.CompleteVideoMultipartUpload;

public sealed record CompleteVideoMultipartUploadCommand(
	string Key,
	string UploadId,
	IReadOnlyList<CompletedPartDto> Parts
) : IRequest<Result>;

public sealed record CompletedPartDto(
	int PartNumber,
	string ETag);