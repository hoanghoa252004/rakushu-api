using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Embedding;

public static class KnowledgeEmbeddingHash
{
	public static string Compute(string text)
	{
		ArgumentNullException.ThrowIfNull(text);

		var bytes = Encoding.UTF8.GetBytes(text);

		var hash = SHA256.HashData(bytes);

		return Convert.ToHexString(hash).ToLowerInvariant();
	}
}