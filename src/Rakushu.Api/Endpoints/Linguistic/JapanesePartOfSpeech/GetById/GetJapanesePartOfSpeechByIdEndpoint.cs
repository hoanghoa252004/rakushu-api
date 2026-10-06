using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.GetById;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.GetById;

internal sealed class GetJapanesePartOfSpeechByIdEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapGet("/{id}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetJapanesePartOfSpeechByIdQuery(id), cancellationToken);
				return result switch
				{
					null => Results.NotFound(),
					_ => Results.Ok(result)
				};
			})
			.WithName("GetJapanesePartOfSpeechById");
	}
}
