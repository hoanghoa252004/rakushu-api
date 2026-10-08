using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.CreateVideoMultipartUpload;

public sealed record CreateVideoMultipartUploadCommand(
	string ContentType
) : IRequest<Result<CreateVideoMultipartUploadResponseDto>>;

public sealed record CreateVideoMultipartUploadResponseDto(
	string UploadId,
	string Key
);