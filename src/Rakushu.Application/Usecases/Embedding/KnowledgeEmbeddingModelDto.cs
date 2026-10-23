using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Embedding;

public sealed record KnowledgeEmbeddingModelDto(
	Guid Id,
	Guid KnowledgeMeaningId,
	string EmbeddingText,
	string ContentHash
	);