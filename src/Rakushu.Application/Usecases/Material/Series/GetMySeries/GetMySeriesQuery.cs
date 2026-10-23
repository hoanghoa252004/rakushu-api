using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Series.GetMySeries;


public sealed record GetMySeriesQuery() : IRequest<Result<IReadOnlyCollection<SeriesDto>>>;