using Entity = Rakushu.Domain.Entities.Video.MediaAsset.MediaAsset;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.MediaAsset.GetMediaAssetById;

public sealed record GetMediaAssetByIdQuery(Guid MediaAssetId) : IRequest<Result<MediaAssetDto>>;
