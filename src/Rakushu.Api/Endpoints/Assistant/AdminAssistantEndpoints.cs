using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Assistant;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Assistant;

internal sealed class GetAdminConversations : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAssistantEndpoints()
			.MapGet("/admin/conversations", async (
				ISender sender,
				[FromQuery] Guid? learnerId,
				[FromQuery] Guid? videoId,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new GetAdminConversationsQuery(learnerId, videoId, pageNumber, pageSize), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetAdminConversations")
			.WithSummary("Admin")
			.WithDescription("Lists learners' assistant conversations (newest activity first). Filter by learnerId and/or videoId.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			.Produces<PaginatedList<AdminConversationDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden);
	}
}

internal sealed class GetAdminConversationMessages : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAssistantEndpoints()
			.MapGet("/admin/conversations/{conversationId:guid}/messages", async (
				[FromRoute] Guid conversationId,
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 20,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new GetAdminConversationMessagesQuery(conversationId, pageNumber, pageSize), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetAdminConversationMessages")
			.WithSummary("Admin")
			.WithDescription("Gets a conversation's messages (newest first, paged). Attachments include a presigned read url.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			.Produces<AdminConversationMessagesDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound);
	}
}
