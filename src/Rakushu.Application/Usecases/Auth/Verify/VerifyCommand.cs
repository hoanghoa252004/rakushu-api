using MediatR;
using Rakushu.Application.Usecases.Auth.Login;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Auth.Verify;

public sealed record VerifyCommand(
	string Email,
	string Code
) : IRequest<Result>;