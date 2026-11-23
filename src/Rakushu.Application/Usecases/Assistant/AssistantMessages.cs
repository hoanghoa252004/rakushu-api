using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.Ai;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Assisstant.Conversation;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Assistant;

// POST (non-streaming, Converse)
public sealed record SendAssistantMessageCommand(
	Guid VideoId,
	Guid TranscriptSegmentId,
	string Content,
	AttachmentInput? Attachment,
	Guid? LearningUnitId = null) : IRequest<Result<AssistantReplyDto>>;

internal sealed class SendAssistantMessageValidator : AbstractValidator<SendAssistantMessageCommand>
{
	public SendAssistantMessageValidator()
	{
		RuleFor(x => x.VideoId).NotEmpty();
		RuleFor(x => x.TranscriptSegmentId).NotEmpty();
		RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
	}
}

internal sealed class SendAssistantMessageHandler(
	AssistantChatService chatService,
	IAiService aiService,
	IStorageService storageService,
	ILogger<SendAssistantMessageHandler> logger)
	: IRequestHandler<SendAssistantMessageCommand, Result<AssistantReplyDto>>
{
	public async Task<Result<AssistantReplyDto>> Handle(
		SendAssistantMessageCommand request, CancellationToken cancellationToken)
	{
		var prepared = await chatService.PrepareAsync(
			request.VideoId, request.TranscriptSegmentId, request.Content, request.Attachment, request.LearningUnitId, cancellationToken);
		if (prepared.IsFailure)
			return Result.Failure<AssistantReplyDto>(prepared.Error);

		AiChatResult? ai = null;
		string? error = null;
		try
		{
			ai = await aiService.ConverseAsync(prepared.Value.AiRequest, cancellationToken);
		}
		catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
		{
			logger.LogError(ex, "Assistant Converse failed for video {VideoId}", request.VideoId);
			error = ex.Message;
		}

		var completed = await chatService.CompleteAsync(
			prepared.Value, ai?.Text, ai?.InputTokens, ai?.OutputTokens, ai?.Blocked ?? false, error, cancellationToken);
		if (completed.IsFailure)
			return Result.Failure<AssistantReplyDto>(completed.Error);

		if (error is not null)
			return Result.Failure<AssistantReplyDto>(ConversationErrors.AiFailed);

		var userMessage = prepared.Value.UserMessage;
		string? url = null;
		return Result.Success(new AssistantReplyDto(
			chatService.ToDto(userMessage, url),
			completed.Value,
			AssistantChatService.ExtractReferences(prepared.Value, completed.Value.Content)));
	}
}

// GET
public sealed record GetAssistantMessagesQuery(
	Guid VideoId,
	DateTime? Before,
	int Limit) : IRequest<Result<IReadOnlyList<MessageDto>>>;

internal sealed class GetAssistantMessagesValidator : AbstractValidator<GetAssistantMessagesQuery>
{
	public GetAssistantMessagesValidator()
	{
		RuleFor(x => x.VideoId).NotEmpty();
		RuleFor(x => x.Limit).InclusiveBetween(1, 100);
	}
}

internal sealed class GetAssistantMessagesHandler(
	ICurrentUserContext currentUser,
	IConversationRepository conversationRepository,
	IStorageService storageService,
	AssistantChatService chatService)
	: IRequestHandler<GetAssistantMessagesQuery, Result<IReadOnlyList<MessageDto>>>
{
	public async Task<Result<IReadOnlyList<MessageDto>>> Handle(
		GetAssistantMessagesQuery request, CancellationToken cancellationToken)
	{
		var learnerId = currentUser.UserId;
		if (learnerId is null)
			return Result.Failure<IReadOnlyList<MessageDto>>(ConversationErrors.UserNotFound);

		var conversation = await conversationRepository.GetByLearnerAndVideoAsync(
			learnerId, VideoId.From(request.VideoId), cancellationToken);
		if (conversation is null)
			return Result.Success<IReadOnlyList<MessageDto>>([]);

		var messages = await conversationRepository.GetMessagesAsync(
			conversation.Id, request.Before, request.Limit, cancellationToken);

		var result = new List<MessageDto>(messages.Count);
		foreach (var m in messages)
		{
			string? url = m.Attachment is null
				? null
				: await storageService.CreatePresignedReadUrlAsync(m.Attachment.FileKey, cancellationToken);
			result.Add(chatService.ToDto(m, url));
		}

		return Result.Success<IReadOnlyList<MessageDto>>(result);
	}
}
