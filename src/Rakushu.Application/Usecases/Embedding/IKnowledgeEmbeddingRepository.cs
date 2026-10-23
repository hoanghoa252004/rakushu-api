//using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Rakushu.Application.Usecases.Embedding;

//public interface IKnowledgeEmbeddingRepository
//{
//	Task<KnowledgeEmbeddingModelDto?> GetByMeaningIdAsync(
//		KnowledgeMeaningId knowledgeMeaningId,
//		CancellationToken cancellationToken = default);

//	Task AddKnowledgeEmbedding(
//		KnowledgeMeaning meaning,
//		KnowledgeEmbeddingModelDto? knowledgeEmbeddingModelDto,
//		float[] embeddingVector,
//		string embeddingText,
//		string contentHash,
//		CancellationToken cancellationToken = default);

//	Task<IReadOnlyList<KnowledgeRetrievalResult>> SearchSimilarAsync(
//		float[] queryEmbedding,
//		int topK,
//		CancellationToken cancellationToken = default);
//}

//public sealed record KnowledgeRetrievalResult(
//	Guid KnowledgeMeaningId,
//	Guid KnowledgeUnitId,
//	string Expression,
//	string Reading,
//	string Meaning,
//	string? Description,
//	string CollectionName,
//	double Similarity);