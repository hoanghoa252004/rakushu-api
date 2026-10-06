using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.GetById;

namespace Rakushu.Api.Endpoints.Linguistic.JapaneseConjugationForm.GetById;

internal sealed class GetJapaneseConjugationFormByIdEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapGet("/{id}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetJapaneseConjugationFormByIdQuery(id);
				var result = await sender.Send(query, cancellationToken);
				return result is null ? Results.NotFound() : Results.Ok(result);
			})
			.WithName("GetJapaneseConjugationFormById");
	}
}
