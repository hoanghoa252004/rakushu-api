using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.JapaneseConjugationForm.CreateJapaneseConjugationForm;

namespace Rakushu.Api.Endpoints.JapaneseConjugationForm.CreateJapaneseConjugationForm;

internal sealed class CreateJapaneseConjugationForm : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapPost("/", async ([FromBody] CreateJapaneseConjugationFormCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetJapaneseConjugationFormById", id => new { id });
			})
			.WithName("CreateJapaneseConjugationForm");
	}
}
