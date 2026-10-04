using Entity = Rakushu.Domain.Entities.Video.MediaAsset.MediaAsset;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Learning.MediaAsset;

namespace Rakushu.Application.Usecases.Learning.MediaAsset.GetMediaAssets;

public sealed record GetMediaAssetsQuery : IRequest<Result<IReadOnlyCollection<MediaAssetDto>>>;
