using FluentValidation;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategories;

public sealed class GetContentCategoriesValidator : AbstractValidator<GetContentCategoriesQuery>
{
	public GetContentCategoriesValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThanOrEqualTo(1)
			.WithMessage("Page number must be greater than or equal to 1");

		RuleFor(x => x.PageSize)
			.GreaterThanOrEqualTo(1)
			.LessThanOrEqualTo(100)
			.WithMessage("Page size must be between 1 and 100");
	}
}
