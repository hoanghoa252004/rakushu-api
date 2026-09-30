using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.JapaneseConjugationForm.UpdateJapaneseConjugationForm;

namespace Rakushu.Api.Endpoints.JapaneseConjugationForm.UpdateJapaneseConjugationForm;

internal sealed class UpdateJapaneseConjugationForm : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateJapaneseConjugationFormCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { JapaneseConjugationFormId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateJapaneseConjugationForm");
	}
}
