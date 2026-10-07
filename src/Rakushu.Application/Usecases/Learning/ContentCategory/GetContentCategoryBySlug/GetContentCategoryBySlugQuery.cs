using MediatR;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryBySlug;

public sealed record GetContentCategoryBySlugQuery(
	string Slug
	) : IRequest<Result<ContentCategoryDto>>;