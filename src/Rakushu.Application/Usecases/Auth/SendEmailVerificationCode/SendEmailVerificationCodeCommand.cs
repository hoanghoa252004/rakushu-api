using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rakushu.Application.Usecases.Auth.SendEmailVerificationCode;

public sealed record SendEmailVerificationCodeCommand(
	string Email
	) : IRequest<Result>;
