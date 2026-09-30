using Entity = Rakushu.Domain.Entities.Video.MediaAsset.MediaAsset;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.MediaAsset.GetMediaAssets;

public sealed record GetMediaAssetsQuery : IRequest<Result<IReadOnlyCollection<MediaAssetDto>>>;
