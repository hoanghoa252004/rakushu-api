using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Assisstant.Conversation;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Assistant;

public sealed record AdminConversationDto(
	Guid Id,
	Guid LearnerId,
	string LearnerName,
	string LearnerEmail,
	Guid VideoId,
	string VideoTitle,
	string Title,
	DateTime CreatedAt,
	DateTime UpdatedAt);

public sealed record AdminConversationMessagesDto(
	AdminConversationDto Conversation,
	PaginatedList<MessageDto> Messages);

internal static class AdminConversationMapper
{
	internal static AdminConversationDto ToDto(Conversation c) => new(
		c.Id.Value,
		c.LearnerId.Value,
		c.Learner.FullName,
		c.Learner.Email,
		c.VideoId.Value,
		c.Video.Title,
		c.Title,
		c.CreatedAt,
		c.UpdatedAt);
}

// List conversations
public sealed record GetAdminConversationsQuery(
	Guid? LearnerId,
	Guid? VideoId,
	int PageNumber,
	int PageSize) : IRequest<Result<PaginatedList<AdminConversationDto>>>;

internal sealed class GetAdminConversationsValidator : AbstractValidator<GetAdminConversationsQuery>
{
	public GetAdminConversationsValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetAdminConversationsHandler(IConversationRepository conversationRepository)
	: IRequestHandler<GetAdminConversationsQuery, Result<PaginatedList<AdminConversationDto>>>
{
	public async Task<Result<PaginatedList<AdminConversationDto>>> Handle(
		GetAdminConversationsQuery request, CancellationToken cancellationToken)
	{
		var (items, total) = await conversationRepository.GetConversationsPagedAsync(
			request.LearnerId is null ? null : UserId.From(request.LearnerId.Value),
			request.VideoId is null ? null : VideoId.From(request.VideoId.Value),
			request.PageNumber,
			request.PageSize,
			cancellationToken);

		var dtos = items.Select(AdminConversationMapper.ToDto).ToList();
		return Result.Success(PaginatedList<AdminConversationDto>.Create(dtos, total, request.PageNumber, request.PageSize));
	}
}

// Paged messages of one conversation (newest first) with presigned attachment urls
public sealed record GetAdminConversationMessagesQuery(
	Guid ConversationId,
	int PageNumber,
	int PageSize) : IRequest<Result<AdminConversationMessagesDto>>;

internal sealed class GetAdminConversationMessagesValidator : AbstractValidator<GetAdminConversationMessagesQuery>
{
	public GetAdminConversationMessagesValidator()
	{
		RuleFor(x => x.ConversationId).NotEmpty();
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetAdminConversationMessagesHandler(
	IConversationRepository conversationRepository,
	IStorageService storageService,
	AssistantChatService chatService)
	: IRequestHandler<GetAdminConversationMessagesQuery, Result<AdminConversationMessagesDto>>
{
	public async Task<Result<AdminConversationMessagesDto>> Handle(
		GetAdminConversationMessagesQuery request, CancellationToken cancellationToken)
	{
		var conversation = await conversationRepository.GetConversationDetailAsync(
			Domain.Entities.Assisstant.Conversation.ConversationId.From(request.ConversationId), cancellationToken);
		if (conversation is null)
			return Result.Failure<AdminConversationMessagesDto>(ConversationErrors.ConversationNotFound);

		var (messages, total) = await conversationRepository.GetMessagesPagedAsync(
			conversation.Id, request.PageNumber, request.PageSize, cancellationToken);

		var dtos = new List<MessageDto>(messages.Count);
		foreach (var m in messages)
		{
			string? url = m.Attachment is null
				? null
				: await storageService.CreatePresignedReadUrlAsync(m.Attachment.FileKey, cancellationToken);
			dtos.Add(chatService.ToDto(m, url));
		}

		return Result.Success(new AdminConversationMessagesDto(
			AdminConversationMapper.ToDto(conversation),
			PaginatedList<MessageDto>.Create(dtos, total, request.PageNumber, request.PageSize)));
	}
}
