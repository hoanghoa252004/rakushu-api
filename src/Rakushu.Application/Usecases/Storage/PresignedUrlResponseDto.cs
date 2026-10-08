using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Storage;

public sealed record PresignedUrlResponseDto(
	string Key,
	string PresignUrl
	);
