using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Payment.ExpirePayments;

public sealed record ExpirePaymentsCommand: IRequest<Result>;
