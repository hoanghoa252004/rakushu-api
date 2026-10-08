using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.AbortVideoMultipartUpload;

public sealed record AbortVideoMultipartUploadCommand(
	string Key,
	string UploadId
) : IRequest<Result>;
