using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Admin.Users.GetUsers;

internal sealed class GetUsersValidator : AbstractValidator<GetUsersQuery>
{
	public GetUsersValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThanOrEqualTo(1);

		RuleFor(x => x.PageSize)
			.InclusiveBetween(1, 100);
	}
}