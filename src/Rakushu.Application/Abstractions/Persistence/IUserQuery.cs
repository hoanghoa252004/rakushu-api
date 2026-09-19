using Rakushu.Application.Usecases.Users.GetUserById;
using Rakushu.Application.Usecases.Users.GetUsers;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IUserQuery
{
	Task<UserDto?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
	Task<(IReadOnlyCollection<UserDto> Items, int TotalCount)> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default);
}
