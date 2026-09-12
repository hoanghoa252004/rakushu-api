using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.Storage;

public interface IStorageService
{
	Task<(string Key, string PresignUrl)> CreatePresignedUrlAsync(string contentType, CancellationToken cancellationToken);
	Task<string> CreatePresignedReadUrlAsync(string key, CancellationToken cancellationToken);
}
