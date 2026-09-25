using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Authentication;

public sealed class WorkerCurrentUserContext : ICurrentUserContext
{
	public UserId? UserId => throw new NotImplementedException();

	public string? Role => throw new NotImplementedException();
}
