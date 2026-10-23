using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Video.DeleteVideo;

public sealed record DeleteVideoCommand(Guid VideoId) : IRequest<Result>;
