using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Series;

public sealed class SeriesId : StronglyTypedId<Guid>
{
	private SeriesId(Guid value) : base(value)
	{
	}

	public static SeriesId Create() => new(Guid.NewGuid());

	public static SeriesId From(Guid value) => new(value);
}