namespace Rakushu.Application.Abstractions.Infrastructure.Authentication;

public interface ICurrentUserContext
{
	Guid UserId { get; }
	string Role { get; }
}
