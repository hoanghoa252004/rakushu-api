using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Nlp.KnowledgeNlpProcess;

public sealed record KnowledgeNlpProcessCommand : IRequest<Result>;
