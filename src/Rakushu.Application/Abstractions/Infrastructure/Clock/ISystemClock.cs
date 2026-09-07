using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.Clock;

public interface ISystemClock
{
	DateTimeOffset UtcNow { get; }
}
