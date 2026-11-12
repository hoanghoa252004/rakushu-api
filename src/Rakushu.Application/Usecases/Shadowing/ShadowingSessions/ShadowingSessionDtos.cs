using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingSessions;

public sealed record SessionUserDto(Guid Id, string FullName, string Email);

public sealed record SessionVideoDto(Guid Id, string Title, string Slug, string? Description, TimeSpan Duration);

public sealed record SessionMaterialDto(
	Guid Id,
	string Name,
	ShadowingMaterialStatus Status,
	int Version,
	SessionVideoDto Video);

public sealed record SessionRoleDto(Guid Id, string SpeakerName, string? AvatarKey, string? AvatarUrl);

public sealed record SessionMemberDto(
	Guid Id,
	SessionUserDto User,
	SessionMemberJoinType JoinType,
	Guid? ShadowingRoleId,
	DateTimeOffset JoinedAt,
	SessionRoleDto? Role);

public static class SessionAvatarResolver
{
	public static async Task<IReadOnlyDictionary<string, string>> ResolveAsync(
		IEnumerable<ShadowingSession> sessions,
		IStorageService storage,
		CancellationToken cancellationToken)
	{
		var keys = sessions
			.SelectMany(s => s.Members)
			.Select(m => m.ShadowingRole?.Speaker?.AvatarKey)
			.Where(k => !string.IsNullOrWhiteSpace(k))
			.Distinct()
			.ToList();

		var result = new Dictionary<string, string>();
		foreach (var key in keys)
			result[key!] = await storage.CreatePresignedReadUrlAsync(key!, cancellationToken);
		return result;
	}
}

public sealed record ShadowingSessionDto(
	Guid Id,
	string Title,
	ShadowingSessionVisibility Visibility,
	ShadowingSessionStatus Status,
	DateTimeOffset ScheduledStartAt,
	DateTimeOffset? StartedAt,
	DateTimeOffset? EndedAt,
	SessionUserDto? Host,
	SessionMaterialDto Material,
	IReadOnlyList<SessionMemberDto> Members,
	int MaxMemberNumbers,
	int CurrentMemberNumbers)
{
	// Requires ShadowingMaterial(+Video) and Members(+Member) to be loaded.
	public static ShadowingSessionDto FromEntity(
		ShadowingSession s, IReadOnlyDictionary<string, string>? avatarUrls = null)
	{
		var members = s.Members
			.OrderBy(m => m.JoinedAt)
			.Select(m => new SessionMemberDto(
				m.Id.Value,
				new SessionUserDto(m.Member.Id.Value, m.Member.FullName, m.Member.Email),
				m.JoinType,
				m.ShadowingRoleId?.Value,
				m.JoinedAt,
				m.ShadowingRole is null
					? null
					: new SessionRoleDto(
						m.ShadowingRole.Id.Value,
						m.ShadowingRole.Speaker.Name,
						m.ShadowingRole.Speaker.AvatarKey,
						m.ShadowingRole.Speaker.AvatarKey is not null
							&& avatarUrls is not null
							&& avatarUrls.TryGetValue(m.ShadowingRole.Speaker.AvatarKey, out var url)
							? url : null)))
			.ToList();

		var host = members.FirstOrDefault(m => m.User.Id == s.HostId.Value)?.User;
		var video = s.ShadowingMaterial.Video;

		return new ShadowingSessionDto(
			s.Id.Value,
			s.Title,
			s.Visibility,
			s.Status,
			s.ScheduledStartAt,
			s.StartedAt,
			s.EndedAt,
			host,
			new SessionMaterialDto(
				s.ShadowingMaterial.Id.Value,
				s.ShadowingMaterial.Name,
				s.ShadowingMaterial.Status,
				s.ShadowingMaterial.Version,
				new SessionVideoDto(video.Id.Value, video.Title, video.Slug, video.Description, video.Duration)),
			members,
			s.ShadowingMaterial.Roles.Count,
			members.Count);
	}
}

public sealed record ShadowingSessionInvitationDto(
	Guid Id,
	Guid SessionId,
	string? SessionTitle,
	DateTimeOffset? SessionScheduledStartAt,
	Guid HostId,
	Guid LearnerId,
	SessionInvitationStatus Status,
	DateTimeOffset SentAt,
	DateTimeOffset ExpiredAt,
	DateTimeOffset? ResponsedAt)
{
	public static ShadowingSessionInvitationDto FromEntity(
		ShadowingSessionInvitation i, ShadowingSession? session = null) => new(
		i.Id.Value,
		i.ShadowingSessionId.Value,
		session?.Title,
		session?.ScheduledStartAt,
		i.HostId.Value,
		i.LearnerId.Value,
		i.Status,
		i.SentAt,
		i.ExpiredAt,
		i.ResponsedAt);
}
