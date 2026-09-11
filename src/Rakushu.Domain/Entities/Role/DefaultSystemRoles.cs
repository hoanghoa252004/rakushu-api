using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Role;

public static class DefaultSystemRoles
{
	public static readonly string SystemAdministrator = nameof(SystemAdministrator);
	public static readonly string LinguisticCurator = nameof(LinguisticCurator);
	public static readonly string Learner = nameof(Learner);
}
