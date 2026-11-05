using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestionById;

public sealed record GetBankQuestionByIdQuery(Guid Id) : IRequest<Result<BankQuestionDetailDto>>;
