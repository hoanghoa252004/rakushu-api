using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Extensions.Options;

public sealed class VnPaySettings
{
	public const string ConfigurationSection = nameof(VnPaySettings);
	public string TmnCode { get; set; } = string.Empty;
	public string HashSecret { get; set; } = string.Empty;
	public string BaseUrl { get; set; } = string.Empty;
	public string ReturnUrl { get; set; } = string.Empty;
	public string Command { get; set; } = string.Empty;
	public string CurrCode { get; set; } = string.Empty;
	public string Locale { get; set; } = string.Empty;
	public string OrderType { get; set; } = string.Empty;
	public string BankCode { get; set; } = string.Empty;
	public int PaymentExpiredInSeconds { get; set; } = 900;
	public int TransactionExpiredInSeconds { get; set; } = 300;
}
