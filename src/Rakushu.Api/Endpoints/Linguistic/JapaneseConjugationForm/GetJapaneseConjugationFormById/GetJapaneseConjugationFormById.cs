using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.GetJapaneseConjugationFormById;

namespace Rakushu.Api.Endpoints.Linguistic.JapaneseConjugationForm.GetJapaneseConjugationFormById;

internal sealed class GetJapaneseConjugationFormById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetJapaneseConjugationFormByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetJapaneseConjugationFormById");
	}
}
