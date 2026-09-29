using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.ContentCategory;

public sealed class ContentCategoryId : StronglyTypedId<Guid>
{
	private ContentCategoryId(Guid value) : base(value)
	{
	}

	public static ContentCategoryId Create() => new(Guid.NewGuid());

	public static ContentCategoryId From(Guid value) => new(value);
}