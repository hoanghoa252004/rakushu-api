using Entity = Rakushu.Domain.Entities.Video.MediaAsset.MediaAsset;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video.MediaAsset;

namespace Rakushu.Application.Usecases.MediaAsset.DeleteMediaAsset;

public sealed record DeleteMediaAssetCommand(Guid MediaAssetId) : IRequest<Result>;
