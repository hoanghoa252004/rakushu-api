using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuestionType.GetQuestionTypes;

public sealed record GetQuestionTypesQuery : IRequest<Result<IReadOnlyList<QuestionTypeDto>>>;
