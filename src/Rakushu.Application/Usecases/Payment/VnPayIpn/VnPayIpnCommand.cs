using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Payment.VnPayIpn;

public sealed record VnPayIpnCommand(
	IReadOnlyDictionary<string, string> Parameters
) : IRequest<Result>;