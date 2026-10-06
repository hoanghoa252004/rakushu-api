using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.GetAll;

namespace Rakushu.Api.Endpoints.Linguistic.JapaneseConjugationForm.GetAll;

internal sealed class GetAllJapaneseConjugationFormsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetAllJapaneseConjugationFormsQuery();
				var result = await sender.Send(query, cancellationToken);
				return Results.Ok(result);
			})
			.WithName("GetAllJapaneseConjugationForms");
	}
}
