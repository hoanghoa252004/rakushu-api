using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Persistence.DbContext;

public class RakushuDbContext : Microsoft.EntityFrameworkCore.DbContext
{
	public RakushuDbContext(DbContextOptions<RakushuDbContext> options) : base(options)
	{
	}

	public DbSet<Role> Roles => Set<Role>();
	public DbSet<User> Users => Set<User>();
	public DbSet<Profile> Profiles => Set<Profile>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(RakushuDbContext).Assembly);
	}
}
