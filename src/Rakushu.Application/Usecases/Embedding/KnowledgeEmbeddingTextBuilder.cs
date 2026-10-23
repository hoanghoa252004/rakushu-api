using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Embedding;


public static class KnowledgeEmbeddingTextBuilder
{
	public static string Build(KnowledgeMeaning meaning)
	{
		ArgumentNullException.ThrowIfNull(meaning);

		var unit = meaning.KnowledgeUnit
			?? throw new InvalidOperationException(
				"KnowledgeMeaning must have KnowledgeUnit loaded.");

		var parts = new List<string>
		{
			$"{unit.Expression}",
			//$"表現: {unit.Expression}",
			//$"読み方: {unit.Reading}",
			//$"意味: {meaning.JapaneseMeaning}"
		};

		return string.Join("\n", parts);
	}
}
