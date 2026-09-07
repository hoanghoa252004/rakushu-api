using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;

namespace Rakushu.Infrastructure.Authentication;

public sealed class CurrentUserContext : ICurrentUserContext
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public Guid UserId
	{
		get
		{
			var user = _httpContextAccessor.HttpContext!.User;

			var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

			Guid.TryParse(sub, out var userId);

			return userId;
		}
	}

	public string? Role =>
		_httpContextAccessor.HttpContext!.User?.FindFirst(ClaimTypes.Role)?.Value
		?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value;
}
