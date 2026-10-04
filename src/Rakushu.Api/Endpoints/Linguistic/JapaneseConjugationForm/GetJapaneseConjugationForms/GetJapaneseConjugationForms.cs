using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.GetJapaneseConjugationForms;

namespace Rakushu.Api.Endpoints.Linguistic.JapaneseConjugationForm.GetJapaneseConjugationForms;

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
