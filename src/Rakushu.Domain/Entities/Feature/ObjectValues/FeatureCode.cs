using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Feature.ObjectValues;

public sealed class FeatureCode : ValueObject
{
	public string Value { get; private set;  }

	private FeatureCode(string value)
	{
		Value = value;
	}

	public static Result<FeatureCode> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Failure<FeatureCode>(FeatureErrors.InvalidCode);

		value = value.Trim().ToUpperInvariant();

		if (!Regex.IsMatch(value, "^[A-Z0-9_]+$"))
			return Result.Failure<FeatureCode>(FeatureErrors.InvalidCode);

		if (value.Length > 50)
			return Result.Failure<FeatureCode>(FeatureErrors.InvalidCode);

		return Result.Success(new FeatureCode(value));
	}

	protected override IEnumerable<object?> GetEqualityComponents()
	{
		yield return Value;
	}
}
