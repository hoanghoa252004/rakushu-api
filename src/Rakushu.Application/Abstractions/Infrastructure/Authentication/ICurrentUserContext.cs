using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Abstractions.Infrastructure.Authentication;

public interface ICurrentUserContext
{
	UserId? UserId { get; }
	string? Role { get; }
}
