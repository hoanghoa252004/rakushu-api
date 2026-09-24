using Rakushu.Application.Abstractions.Infrastructure.PaymentGateway;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.Payment;

public interface IPaymentService
{
	Provider Provider { get; }
	DateTimeOffset GetPaymentExpiration();
	DateTimeOffset GetTransactionExpiration();
	Result<string> CreatePaymentUrl(CreatePaymentUrlParams parameters, CancellationToken cancellationToken = default);
	Result ValidateSignature(IReadOnlyDictionary<string, string> parameters, string secureHash, CancellationToken cancellationToken = default);
	Result<string?> GetResponseData(IReadOnlyDictionary<string, string> parameters, string key, CancellationToken cancellationToken = default);
}

public sealed record CreatePaymentUrlParams(
	string IpAddress,
	decimal Amount,
	string Description,
	string TxnRef,
	DateTime CreatedAt,
	DateTime ExpiredAt
);
