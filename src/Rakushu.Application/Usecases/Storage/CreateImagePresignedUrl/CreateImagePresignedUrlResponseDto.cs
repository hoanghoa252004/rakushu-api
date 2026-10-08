using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage.CreateImagePresignedUrl;

public sealed record CreateImagePresignedUrlResponseDto(
	string Key,
	string PresignUrl
	);
