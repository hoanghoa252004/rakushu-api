using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Plan.ChangePlanStatus;

public sealed record ChangePlanStatusCommand(Guid PlanId, PlanStatus Status) : IRequest<Result>;
