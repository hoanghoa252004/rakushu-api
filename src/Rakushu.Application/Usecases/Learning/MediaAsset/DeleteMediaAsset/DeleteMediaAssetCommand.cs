using Entity = Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAsset;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.MediaAsset.DeleteMediaAsset;

public sealed record DeleteMediaAssetCommand(Guid MediaAssetId) : IRequest<Result>;
