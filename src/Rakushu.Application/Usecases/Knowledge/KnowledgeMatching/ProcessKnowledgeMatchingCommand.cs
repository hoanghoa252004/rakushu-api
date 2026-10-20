using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMatching;

public sealed record ProcessKnowledgeMatchingCommand(
	Guid TranscriptSegmentId)
	: IRequest<Result<IReadOnlyList<KnowledgeMatchCandidate>>>;