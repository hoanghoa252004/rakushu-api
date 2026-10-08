namespace Rakushu.Api.Endpoints.Storage;

internal static class StorageEndpointGroupExtension
{
	internal static RouteGroupBuilder MapStorageEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/presigned-url")
			.WithTags("Storage")
			.WithGroupName("storage");
	}
}
