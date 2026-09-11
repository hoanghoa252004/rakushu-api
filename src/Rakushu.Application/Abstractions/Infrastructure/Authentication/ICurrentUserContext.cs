using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Abstractions.Infrastructure.Authentication;

public interface ICurrentUserContext
{
	UserId UserId { get; }
	string RoleTitle { get; }
}
