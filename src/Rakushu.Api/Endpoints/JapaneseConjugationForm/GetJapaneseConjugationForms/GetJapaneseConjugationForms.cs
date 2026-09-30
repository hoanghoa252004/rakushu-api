using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.JapaneseConjugationForm.GetJapaneseConjugationForms;

namespace Rakushu.Api.Endpoints.JapaneseConjugationForm.GetJapaneseConjugationForms;

internal sealed class GetJapaneseConjugationForms : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetJapaneseConjugationFormsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetJapaneseConjugationForms");
	}
}
