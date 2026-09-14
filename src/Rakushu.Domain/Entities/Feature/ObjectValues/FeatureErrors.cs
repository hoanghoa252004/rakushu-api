using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Feature.ObjectValues;

public static class FeatureErrors
{
	public static readonly Error InvalidCode = Error.Validation(
		"FEATURE.INVALID_CODE", "The feature code is invalid, it must have value, not exceed 50 chars, all uppercase, and concat with '_'. For example: AI_CHAT ; QUIZ_GENERATION.");
}