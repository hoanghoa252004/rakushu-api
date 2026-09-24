using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.ProcessSepayWebhook;

public sealed record ProcessSepayWebhookCommand(
	SepayWebhookPayload Payload,
	string? RawData = null,
	string? SignatureHeader = null,
	string? TimestampHeader = null
) : IRequest<Result<bool>>;
