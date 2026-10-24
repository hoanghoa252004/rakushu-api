//using MediatR;
//using Microsoft.Extensions.Logging;
//using Rakushu.Application.Abstractions.Infrastructure.Rag;
//using Rakushu.Domain.Common.Contract;
//using Rakushu.Domain.Common.Results;
//using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Numerics;
//using System.Text;
//using System.Threading.Tasks;

//namespace Rakushu.Application.Usecases.Embedding;


//public sealed class KnowledgeEmbeddingCommandHandler : IRequestHandler<KnowledgeEmbeddingCommand, Result>
//{

//	private readonly IEmbeddingService _knowledgeEmbeddingService;
//	private readonly IUnitOfWork _unitOfWork;
//	private readonly IKnowledgeMeaningRepository _knowledgeMeaningRepository;
//	private readonly IKnowledgeEmbeddingRepository _knowledgeEmbeddingRepository;
//	private const int Dimensions = 1024;
//	private const int BatchSize = 20;

//	public KnowledgeEmbeddingCommandHandler(
//		IEmbeddingService knowledgeEmbeddingService,
//		IUnitOfWork unitOfWork,
//		IKnowledgeMeaningRepository knowledgeMeaningRepository,
//		IKnowledgeEmbeddingRepository knowledgeEmbeddingRepository)
//	{
//		_knowledgeEmbeddingService = knowledgeEmbeddingService;
//		_unitOfWork = unitOfWork;
//		_knowledgeMeaningRepository = knowledgeMeaningRepository;
//		_knowledgeEmbeddingRepository = knowledgeEmbeddingRepository;
//	}	

//	public async Task<Result> Handle(
//		KnowledgeEmbeddingCommand request,
//		CancellationToken cancellationToken)
//	{
//		return await _unitOfWork.ExecuteAsync(async () =>
//		{
//			var offset = 0;

//			while (!cancellationToken.IsCancellationRequested)
//			{
//				var meanings = await _knowledgeMeaningRepository.GetActiveMeaningsForEmbeddingAsync(
//					offset,
//					BatchSize,
//					cancellationToken);

//				if (meanings.Count == 0)
//				{
//					break;
//				}

//				foreach (var meaning in meanings)
//				{
//					cancellationToken.ThrowIfCancellationRequested();

//					try
//					{
//						await ProcessMeaningAsync(
//							meaning,
//							cancellationToken);
//					}
//					catch (OperationCanceledException)
//						when (cancellationToken.IsCancellationRequested)
//					{
//						throw;
//					}
//					catch (Exception ex)
//					{
//						throw;
//					}
//				}

//				offset += meanings.Count;
//			}
//			return Result.Success();
//		}, cancellationToken);
//	}

//	private async Task ProcessMeaningAsync(
//		KnowledgeMeaning meaning,
//		CancellationToken cancellationToken)
//	{
//		var embeddingText =
//			KnowledgeEmbeddingTextBuilder.Build(meaning);

//		var contentHash =
//			KnowledgeEmbeddingHash.Compute(embeddingText);

//		var existing = await _knowledgeEmbeddingRepository.GetByMeaningIdAsync(
//			meaning.Id,
//			cancellationToken);

//		// Skip nếu nội dung không đổi.
//		if (existing is not null && existing.ContentHash == contentHash)
//			return;

//		// Chỉ gọi Bedrock khi chưa có embedding hoặc nội dung đã đổi.
//		var values = await _knowledgeEmbeddingService.GenerateEmbeddingAsync(
//			embeddingText,
//			cancellationToken);

//		if (values.Length != Dimensions)
//		{
//			throw new InvalidOperationException(
//				$"Expected {Dimensions} dimensions, but received {values.Length}.");
//		}

//		await _knowledgeEmbeddingRepository.AddKnowledgeEmbedding(
//			meaning,
//			null,
//			values,
//			embeddingText,
//			contentHash,
//			cancellationToken);
//	}
//}