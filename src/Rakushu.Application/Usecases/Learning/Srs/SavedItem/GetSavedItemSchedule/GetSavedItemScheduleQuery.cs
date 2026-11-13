using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSavedItemSchedule;

public sealed record GetSavedItemScheduleQuery(Guid Id) : IRequest<Result<SavedItemScheduleDto>>;
