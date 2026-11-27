using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Authentication.ExpirePasswordResetCodes;

public sealed record ExpirePasswordResetCodesCommand : IRequest<Result>;
