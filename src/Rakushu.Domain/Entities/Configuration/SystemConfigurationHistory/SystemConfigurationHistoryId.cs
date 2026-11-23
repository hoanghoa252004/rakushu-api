using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Configuration.SystemConfigurationHistory;

public sealed class SystemConfigurationHistoryId : StronglyTypedId<Guid>
{
	private SystemConfigurationHistoryId(Guid value) : base(value)
	{
	}

	public static SystemConfigurationHistoryId Create() => new(Guid.NewGuid());

	public static SystemConfigurationHistoryId From(Guid value) => new(value);
}