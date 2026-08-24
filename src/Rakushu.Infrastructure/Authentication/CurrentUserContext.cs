using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Rakushu.Application.Abstractions.Authentication;

namespace Rakushu.Infrastructure.Authentication;

public sealed class CurrentUserContext : ICurrentUserContext
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public Guid? UserId
	{
		get
		{
			var user = _httpContextAccessor.HttpContext?.User;
			var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
				?? user?.FindFirst("sub")?.Value;

			return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
		}
	}

	public string? Email =>
		_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
		?? _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

	public string? Role =>
		_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value
		?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value;

	public bool IsAuthenticated =>
		_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
