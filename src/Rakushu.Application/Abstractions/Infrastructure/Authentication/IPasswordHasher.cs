namespace Rakushu.Application.Abstractions.Infrastructure.Authentication;

public interface IPasswordHasher
{
	string HashPassword(string password);
	bool VerifyPassword(string password, string passwordHash);
}
