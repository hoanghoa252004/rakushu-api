using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Learning.MediaAsset.CreateMediaAsset;

public sealed record CreateMediaAssetCommand(
	Guid videoId,
	MediaAssetType assetType,
	string storageKey,
	string contentType,
	string fileName,
	long fileSizeBytes
) : IRequest<Result<Guid>>;
