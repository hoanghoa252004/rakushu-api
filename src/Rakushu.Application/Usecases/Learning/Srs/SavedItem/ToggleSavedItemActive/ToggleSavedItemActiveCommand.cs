using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.ToggleSavedItemActive;

public sealed record ToggleSavedItemActiveCommand(Guid Id, bool IsActive) : IRequest<Result>;
