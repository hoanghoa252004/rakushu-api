using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Authentication.SendPasswordResetCode;

public sealed record SendPasswordResetCodeCommand(string Email) : IRequest<Result>;
