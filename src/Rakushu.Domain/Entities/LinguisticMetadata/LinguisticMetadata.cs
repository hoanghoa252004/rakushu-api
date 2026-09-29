using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticMetadata;

public abstract class LinguisticMetadata<TId> : AggregateRoot<TId>
	where TId : notnull
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string VietnameseName { get; private set; } = null!;
	public string? Description { get; private set; }

	protected LinguisticMetadata(
		TId id,
		string code,
		string name,
		string vietnameseName,
		string? description)
		: base(id)
	{
		Code = code;
		Name = name;
		VietnameseName = vietnameseName;
		Description = description;
	}

	protected LinguisticMetadata()
	{
	}

	public void Update(
		string name,
		string vietnameseName,
		string? description)
	{
		Name = name;
		VietnameseName = vietnameseName;
		Description = description;
	}
}