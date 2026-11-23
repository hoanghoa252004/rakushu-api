using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Assistant;
using Rakushu.Application.Abstractions.Infrastructure.Ai;
using Rakushu.Domain.Entities.Assisstant.Conversation;
using Rakushu.Domain.Entities.Identity.Role;
using System.Text;
using System.Text.Json;

namespace Rakushu.Api.Endpoints.Assistant;

internal static class AssistantEndpointGroupExtension
{
	internal static RouteGroupBuilder MapAssistantEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/assistant")
			.WithTags("Assistant")
			.WithGroupName("assistant");
	}
}

public sealed record SendAssistantMessageRequest(
	Guid TranscriptSegmentId,
	string Content,
	AttachmentInput? Attachment,
	Guid? LearningUnitId = null);

internal sealed class SendAssistantMessage : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAssistantEndpoints()
			.MapPost("/videos/{videoId:guid}/messages", async (
				[FromRoute] Guid videoId,
				[FromBody] SendAssistantMessageRequest request,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new SendAssistantMessageCommand(
					videoId, request.TranscriptSegmentId, request.Content, request.Attachment, request.LearningUnitId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("SendAssistantMessage")
			.WithSummary("Learner")
			.WithDescription("Asks the AI assistant a question about a transcript segment, optionally with an uploaded image attachment (S3 file key).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<AssistantReplyDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class StreamAssistantMessage : IEndpoint
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAssistantEndpoints()
			.MapPost("/videos/{videoId:guid}/messages/stream", async (
				[FromRoute] Guid videoId,
				[FromBody] SendAssistantMessageRequest request,
				HttpContext httpContext,
				AssistantChatService chatService,
				IAiService aiService,
				ILogger<StreamAssistantMessage> logger,
				CancellationToken cancellationToken) =>
			{
				var prepared = await chatService.PrepareAsync(
					videoId, request.TranscriptSegmentId, request.Content, request.Attachment, request.LearningUnitId, cancellationToken);
				if (prepared.IsFailure)
					return prepared.Problem();

				var response = httpContext.Response;
				response.StatusCode = StatusCodes.Status200OK;
				response.Headers.ContentType = "text/event-stream";
				response.Headers.CacheControl = "no-cache";
				response.Headers.Append("X-Accel-Buffering", "no");

				async Task Send(string eventName, object data)
				{
					var json = JsonSerializer.Serialize(data, JsonOptions);
					await response.WriteAsync($"event: {eventName}\ndata: {json}\n\n", cancellationToken);
					await response.Body.FlushAsync(cancellationToken);
				}

				await Send("user_message", chatService.ToDto(prepared.Value.UserMessage, null));

				var text = new StringBuilder();
				int? inputTokens = null, outputTokens = null;
				var blocked = false;
				string? error = null;
				try
				{
					await foreach (var chunk in aiService.ConverseStreamAsync(prepared.Value.AiRequest, cancellationToken))
					{
						if (!string.IsNullOrEmpty(chunk.TextDelta))
						{
							text.Append(chunk.TextDelta);
							await Send("delta", new { text = chunk.TextDelta });
						}

						if (chunk.IsFinal)
						{
							inputTokens = chunk.InputTokens;
							outputTokens = chunk.OutputTokens;
							blocked = chunk.Blocked;
						}
					}
				}
				catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
				{
					logger.LogError(ex, "Assistant ConverseStream failed for video {VideoId}", videoId);
					error = ex.Message;
				}

				var completed = await chatService.CompleteAsync(
					prepared.Value, text.ToString(), inputTokens, outputTokens, blocked, error, CancellationToken.None);

				if (error is not null)
					await Send("error", new { code = ConversationErrors.AiFailed.Code, message = ConversationErrors.AiFailed.Message });
				else if (completed.IsSuccess)
					await Send("done", new { message = completed.Value, references = AssistantChatService.ExtractReferences(prepared.Value, completed.Value.Content) });

				return Results.Empty;
			})
			.WithName("StreamAssistantMessage")
			.WithSummary("Learner")
			.WithDescription("Asks the AI assistant and streams the answer as Server-Sent Events. Events: user_message (MessageDto), delta ({text}), done ({message, references}), error ({code,message}).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK, contentType: "text/event-stream")
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetAssistantMessages : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAssistantEndpoints()
			.MapGet("/videos/{videoId:guid}/messages", async (
				[FromRoute] Guid videoId,
				[FromQuery] DateTime? before,
				[FromQuery] int? limit,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(
					new GetAssistantMessagesQuery(videoId, before, limit ?? 30), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetAssistantMessages")
			.WithSummary("Learner")
			.WithDescription("Gets the learner's conversation messages for a video (oldest first, newest page; use 'before' to page backwards).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<IReadOnlyList<MessageDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
