using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticMetadata;

public abstract class Linguistic<TId> : AggregateRoot<TId>
	where TId : notnull
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string JapaneseName { get; private set; } = null!;
	public string? Description { get; private set; }

	protected Linguistic(
		TId id,
		string code,
		string name,
		string japaneseName,
		string? description)
		: base(id)
	{
		Code = code;
		Name = name;
		JapaneseName = japaneseName;
		Description = description;
	}

	protected Linguistic()
	{
	}

	public virtual void Update(
		string name,
		string japaneseName,
		string? description)
	{
		Name = name;
		JapaneseName = japaneseName;
		Description = description;
	}
}
