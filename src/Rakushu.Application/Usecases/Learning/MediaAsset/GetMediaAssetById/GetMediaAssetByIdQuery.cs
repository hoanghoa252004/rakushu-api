using Entity = Rakushu.Domain.Entities.Video.MediaAsset.MediaAsset;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Learning.MediaAsset;

namespace Rakushu.Application.Usecases.Learning.MediaAsset.GetMediaAssetById;

public sealed record GetMediaAssetByIdQuery(Guid MediaAssetId) : IRequest<Result<MediaAssetDto>>;
