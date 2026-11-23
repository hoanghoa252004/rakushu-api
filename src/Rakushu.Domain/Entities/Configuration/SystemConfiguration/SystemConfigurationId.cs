using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Configuration.SystemConfiguration;

public sealed class SystemConfigurationId : StronglyTypedId<Guid>
{
	private SystemConfigurationId(Guid value) : base(value)
	{
	}

	public static SystemConfigurationId Create() => new(Guid.NewGuid());

	public static SystemConfigurationId From(Guid value) => new(value);
}