using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

public sealed class JapaneseConjugationFormId : StronglyTypedId<Guid>
{
	private JapaneseConjugationFormId(Guid value) : base(value)
	{
	}

	public static JapaneseConjugationFormId Create() => new(Guid.NewGuid());

	public static JapaneseConjugationFormId From(Guid value) => new(value);
}