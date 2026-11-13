using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.DeleteQuestion;

public sealed record DeleteQuestionCommand(Guid Id) : IRequest<Result>;
