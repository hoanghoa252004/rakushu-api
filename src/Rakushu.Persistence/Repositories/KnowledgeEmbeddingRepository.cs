using Microsoft.EntityFrameworkCore;
using Pgvector;
using Rakushu.Application.Usecases.Embedding;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Persistence.Configurations.Knowledge;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;


public sealed class KnowledgeEmbeddingRepository : IKnowledgeEmbeddingRepository
{
	private readonly RakushuDbContext _context = null!;

	public KnowledgeEmbeddingRepository(RakushuDbContext context)
	{
		_context = context;
	}

	public Task AddKnowledgeEmbedding(
		KnowledgeMeaning meaning, 
		KnowledgeEmbeddingModelDto? knowledgeEmbeddingModelDto, 
		float[] embeddingVector, 
		string embeddingText,
		string contentHash,
		CancellationToken cancellationToken = default)
	{
		var vector = new Vector(embeddingVector);

		if (knowledgeEmbeddingModelDto is null)
		{
			var entity = KnowledgeEmbedding.Create(
				meaning.Id,
				embeddingText,
				contentHash,
				vector);

			_context.KnowledgeEmbeddings.Add(entity);
		}
		else
		{
			var existing = _context.KnowledgeEmbeddings
				.Single(x => x.Id == knowledgeEmbeddingModelDto.Id);

			existing.EmbeddingText = embeddingText;
			existing.ContentHash = contentHash;
			existing.Embedding = vector;
		}

		return Task.CompletedTask;
	}

	public async Task<KnowledgeEmbeddingModelDto?>GetByMeaningIdAsync(
			KnowledgeMeaningId knowledgeMeaningId,
			CancellationToken cancellationToken = default)
	{
		var embbeding = await _context.KnowledgeEmbeddings
			.SingleOrDefaultAsync(
				x =>
					x.KnowledgeMeaningId == knowledgeMeaningId,
				cancellationToken);

		return embbeding is null ? null : new KnowledgeEmbeddingModelDto(
			embbeding.Id,
			embbeding.KnowledgeMeaningId.Value,
			embbeding.EmbeddingText,
			embbeding.ContentHash
			);
	}

	public async Task<IReadOnlyList<KnowledgeRetrievalResult>> SearchSimilarAsync(
	float[] queryEmbedding,
	int topK,
	CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(queryEmbedding);

		if (queryEmbedding.Length != 1024)
		{
			throw new ArgumentException(
				"Query embedding must have 1024 dimensions.",
				nameof(queryEmbedding));
		}

		if (topK <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(topK));
		}

		var queryVector = new Pgvector.Vector(queryEmbedding);

		var results = await (
			from embedding in _context.KnowledgeEmbeddings.AsNoTracking()
			join meaning in _context.KnowledgeMeanings.AsNoTracking()
				on embedding.KnowledgeMeaningId equals meaning.Id
			join unit in _context.KnowledgeUnits.AsNoTracking()
				on meaning.KnowledgeUnitId equals unit.Id
			join collection in _context.KnowledgeCollections.AsNoTracking()
				on unit.KnowledgeCollectionId equals collection.Id
			where unit.IsActive
				&& collection.IsActive
			orderby embedding.Embedding.CosineDistance(queryVector)
			select new
			{
				Meaning = meaning,
				Unit = unit,
				Collection = collection,
				Distance = embedding.Embedding.CosineDistance(queryVector)
			})
			.Take(topK)
			.ToListAsync(cancellationToken);

		return results
			.Select(x => new KnowledgeRetrievalResult(
				x.Meaning.Id.Value,
				x.Unit.Id.Value,
				x.Unit.Expression,
				x.Unit.Reading,
				x.Meaning.Meaning,
				x.Meaning.Description,
				x.Collection.Name,
				1.0 - x.Distance))
			.ToList();
	}
}