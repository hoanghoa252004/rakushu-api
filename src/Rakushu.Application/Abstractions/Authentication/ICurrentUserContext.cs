namespace Rakushu.Application.Abstractions.Authentication;

public interface ICurrentUserContext
{
	Guid? UserId { get; }
	//string? Email { get; }
	string? Role { get; }
	//bool IsAuthenticated { get; }
}
