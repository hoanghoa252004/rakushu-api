using Entity = Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAsset;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Learning.MediaAsset.UpdateMediaAsset;

public sealed record UpdateMediaAssetCommand(
	Guid MediaAssetId,
	Guid videoId,
	MediaAssetType assetType,
	string storageKey,
	string contentType,
	string fileName,
	long fileSizeBytes,
	string? container,
	string? codec,
	int? width,
	int? height,
	double? frameRate
) : IRequest<Result>;
