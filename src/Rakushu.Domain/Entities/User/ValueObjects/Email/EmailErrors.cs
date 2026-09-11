using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.ValueObjects.Email;

public static class EmailErrors
{
	public static readonly Error Empty = Error.Validation(
		"USER.EMAIL.EMPTY", "Email is required.");

	public static readonly Error TooLong = Error.Validation(
		"USER.EMAIL.TOO_LONG", "Email cannot exceed 256 characters.");

	public static readonly Error InvalidFormat = Error.Validation(
		"USER.EMAIL.INVALID_FORMAT", "Email format is invalid.");
}
