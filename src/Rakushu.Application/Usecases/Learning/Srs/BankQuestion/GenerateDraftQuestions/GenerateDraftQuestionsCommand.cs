using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GenerateDraftQuestions;

public sealed record GenerateDraftQuestionsCommand(int BatchSize = 10) : IRequest<Result<int>>;
