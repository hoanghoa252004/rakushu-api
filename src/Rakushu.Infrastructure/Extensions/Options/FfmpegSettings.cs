using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Extensions.Options;

public sealed class FfmpegSettings
{
	public const string ConfigurationSection = nameof(FfmpegSettings);
	public string ExecutablePath { get; set; } = string.Empty;
}
