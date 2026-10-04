using Rakushu.Application.Usecases.User.Role.GetRoles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IRoleQuery
{
	Task<IReadOnlyCollection<GetRolesDto>> GetRoles(CancellationToken cancellationToken = default);
}
