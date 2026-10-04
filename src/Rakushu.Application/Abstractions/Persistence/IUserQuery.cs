using Rakushu.Application.Usecases.User.User.GetUserById;
using Rakushu.Application.Usecases.User.User.GetUsers;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IUserQuery
{
	Task<UserDetailDto?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
	Task<(IReadOnlyCollection<UserListItemDto> Items, int TotalCount)> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default);
}
