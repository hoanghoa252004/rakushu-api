using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.SupportedLanguage;

public sealed class SupportedLanguageId : StronglyTypedId<Guid>
{
	private SupportedLanguageId(Guid value) : base(value)
	{
	}

	public static SupportedLanguageId Create() => new(Guid.NewGuid());

	public static SupportedLanguageId From(Guid value) => new(value);
}