using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using DomainUserId = Rakushu.Domain.Entities.User.UserId;

namespace Rakushu.Infrastructure.Authentication;

public sealed class CurrentUserContext : ICurrentUserContext
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public DomainUserId? UserId
	{
		get
		{
			var value = _httpContextAccessor.HttpContext?
						   .User
						   .FindFirst(ClaimTypes.NameIdentifier)?
						   .Value;

			if (!Guid.TryParse(value, out var userId))
				return null;

			return DomainUserId.From(userId);
		}
	}

	public string? Role
	{
		get
		{
			return _httpContextAccessor.HttpContext?
				.User
				.FindFirst(ClaimTypes.Role)?
				.Value;
		}
	}
}
