using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.ApproveQuestion;

public sealed record ApproveQuestionCommand(Guid Id) : IRequest<Result>;
