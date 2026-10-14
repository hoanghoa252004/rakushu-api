using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Learning.MediaAsset.UpdateMediaAsset;

public sealed record UpdateMediaAssetCommand(
	Guid MediaAssetId,
	Guid VideoId,
	MediaAssetType AssetType,
	string StorageKey,
	string ContentType,
	string FileName,
	long FileSizeBytes
) : IRequest<Result>;
