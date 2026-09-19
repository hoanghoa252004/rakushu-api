using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Feature.ChangeFeatureStatus;

public sealed record ChangeFeatureStatusCommand(Guid FeatureId, string Status) : IRequest<Result>;
