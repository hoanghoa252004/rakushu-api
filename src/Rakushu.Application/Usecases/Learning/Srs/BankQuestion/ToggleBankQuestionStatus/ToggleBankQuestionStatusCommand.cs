using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.ToggleBankQuestionStatus;

public sealed record ToggleBankQuestionStatusCommand(
	Guid Id,
	bool IsActive) : IRequest<Result>;
