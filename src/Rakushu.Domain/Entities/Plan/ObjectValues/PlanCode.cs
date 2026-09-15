using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Plan.ObjectValues;

public sealed class PlanCode : ValueObject
{
	public string Value { get; private set; }

	private PlanCode(string value)
	{
		Value = value;
	}

	public static Result<PlanCode> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Failure<PlanCode>(PlanErrors.InvalidCode);

		value = value.Trim().ToUpperInvariant();

		if (!Regex.IsMatch(value, "^[A-Z0-9_]+$"))
			return Result.Failure<PlanCode>(PlanErrors.InvalidCode);

		if (value.Length > 50)
			return Result.Failure<PlanCode>(PlanErrors.InvalidCode);

		return Result.Success(new PlanCode(value));
	}

	protected override IEnumerable<object?> GetEqualityComponents()
	{
		yield return Value;
	}
}