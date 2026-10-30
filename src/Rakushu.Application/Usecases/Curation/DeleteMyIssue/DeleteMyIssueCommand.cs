using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.DeleteMyIssue;

public sealed record DeleteMyIssueCommand(Guid IssueId) : IRequest<Result>;
