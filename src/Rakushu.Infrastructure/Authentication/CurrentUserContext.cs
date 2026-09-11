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

	public UserId UserId
	{
		get
		{
			var user = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

			Guid.TryParse(user, out var userId);

			return DomainUserId.From(userId);
		}
	}

	public string RoleTitle
	{
		get
		{
			var role = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value;

			return role;
		}
	}
}
