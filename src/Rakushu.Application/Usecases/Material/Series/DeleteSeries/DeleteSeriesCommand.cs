using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Series.DeleteSeries;

public sealed record DeleteSeriesCommand(Guid SeriesId) : IRequest<Result>;
