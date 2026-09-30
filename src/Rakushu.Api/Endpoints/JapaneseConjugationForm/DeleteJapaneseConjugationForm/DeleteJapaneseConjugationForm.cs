using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.JapaneseConjugationForm.DeleteJapaneseConjugationForm;

namespace Rakushu.Api.Endpoints.JapaneseConjugationForm.DeleteJapaneseConjugationForm;

internal sealed class DeleteJapaneseConjugationForm : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteJapaneseConjugationFormCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteJapaneseConjugationForm");
	}
}
